using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NiteCompiler.CodeAnalysis.Declarations;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceModuleSymbol : ModuleSymbol
{
	public MergedModuleDeclaration Declaration { get; }
	public override string Name { get; }
	public override Symbol ContainingSymbol { get; }

	public override SourceLibrarySymbol ContainingLibrary
	{
		get
		{
			if (ContainingSymbol is SourceLibrarySymbol lib)
			{
				return lib;
			}

			if (ContainingSymbol is SourceModuleSymbol module)
			{
				return module.ContainingLibrary;
			}

			throw new UnreachableException("Wrong containing symbol.");
		}
	}

	private CompletionPart _state;

	public SourceModuleSymbol(Symbol containing, MergedModuleDeclaration declaration, string name)
	{
		Declaration = declaration;
		ContainingSymbol = containing;
		Name = name;
	}

	private ImmutableArray<Symbol> _lateinitMembersUnordered;
	public override ImmutableArray<Symbol> GetMembersUnordered()
	{
		if (_lateinitMembersUnordered.IsDefault)
		{
			var members = GetNameToMembersMap().Values.SelectMany(static s => s).ToImmutableArray();
			ImmutableInterlocked.InterlockedInitialize(ref _lateinitMembersUnordered, members);
		}

		return _lateinitMembersUnordered;
	}

	public override ImmutableArray<Symbol> GetMembers()
	{
		// TODO: Order
		return GetMembersUnordered();
	}

	private Dictionary<string, ImmutableArray<Symbol>>? _lateinitNameToMembersMap;
	private Dictionary<string, ImmutableArray<Symbol>> GetNameToMembersMap()
	{
		if (_lateinitNameToMembersMap == null)
		{
			if (Interlocked.CompareExchange(ref _lateinitNameToMembersMap, MakeNameToMembersMap(), null) == null)
			{
				bool wasSetThisThread = _state.NotePartComplete(CompletionPart.NameToMembersMap);
				Debug.Assert(wasSetThisThread);
			}
		}

		return _lateinitNameToMembersMap;
	}

	private Dictionary<string, ImmutableArray<Symbol>> MakeNameToMembersMap()
	{
		Dictionary<string, ImmutableArray<Symbol>> result = new(capacity: Declaration.Members.Length);
		foreach (var declaration in Declaration.Members)
		{
			Symbol symbol = BuildSymbol(declaration);

			if (!result.TryGetValue(symbol.Name, out var existing))
			{
				result[symbol.Name] = [symbol];
			}
			else
			{
				result[symbol.Name] = existing.Add(symbol);
			}
		}

		foreach (var declaration in Declaration.Declarations)
		{
			SyntaxNode moduleSyntax = declaration.SyntaxReference.GetSyntax();

			if (moduleSyntax is ModuleDeclarationSyntax moduleDeclarationSyntax)
			{
				foreach (var memberSyntax in moduleDeclarationSyntax.Members)
				{
					if (memberSyntax.Kind == NodeKind.TypeDeclaration) continue;

					Symbol symbol = BuildSymbol(memberSyntax);

					if (!result.TryGetValue(symbol.Name, out var existing))
					{
						result[symbol.Name] = [symbol];
					}
					else
					{
						result[symbol.Name] = existing.Add(symbol);
					}
				}
			}
			else if (moduleSyntax is CompilationUnitSyntax rootDeclarationSyntax)
			{
				foreach (var memberSyntax in rootDeclarationSyntax.Items)
				{
					if (memberSyntax.Kind == NodeKind.ModuleDeclaration ||
					    memberSyntax.Kind == NodeKind.TypeDeclaration) continue;

					Symbol symbol = BuildSymbol(memberSyntax);

					if (!result.TryGetValue(symbol.Name, out var existing))
					{
						result[symbol.Name] = [symbol];
					}
					else
					{
						result[symbol.Name] = existing.Add(symbol);
					}
				}
			}
		}

		return result;
	}

	private Symbol BuildSymbol(Declaration declaration)
	{
		switch (declaration.Kind)
		{
			case DeclarationKind.Module:
				return new SourceModuleSymbol(this, (MergedModuleDeclaration)declaration, declaration.Name);
			case DeclarationKind.Type:
				return new SourceTypeSymbol(this, (MergedTypeDeclaration)declaration);

			default:
				throw new InvalidEnumArgumentException(nameof(declaration.Kind), (int)declaration.Kind, typeof(DeclarationKind));
		}
	}

	private Symbol BuildSymbol(SyntaxNode syntax)
	{
		if (syntax.Kind == NodeKind.FunctionDeclaration)
		{
			return new SourceFunctionSymbol(this, (FunctionDeclarationSyntax)syntax);
		}
		else
		{
			throw new UnreachableException();
		}
	}

	private void RegisterSpecialTypes()
	{
		foreach (var array in _lateinitNameToMembersMap!.Values)
		{
			foreach (var member in array)
			{
				if (member is TypeSymbol type && type.SpecialType != SpecialType.None)
				{
					DeclaringCompilation!.RegisterSpecialType(type);

					if (!DeclaringCompilation!.LookingForSpecialTypes)
					{
						return;
					}
				}
			}
		}
	}

	internal override void ForceComplete(Predicate<Symbol>? filter, CancellationToken cancellationToken = default)
	{
		if (filter?.Invoke(this) == false)
		{
			return;
		}

		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var incompletePart = _state.NextIncompletePart;
			switch (incompletePart)
			{
				case CompletionPart.NameToMembersMap:
					_ = GetNameToMembersMap();
					break;
				case CompletionPart.MembersCompleted:
					var members = GetMembers();
					if (DeclaringCompilation!.LookingForSpecialTypes)
					{
						RegisterSpecialTypes();
					}

					bool allCompleted = true;

					// TODO: Concurrency
					foreach (var member in members)
					{
						ForceCompleteMemberConditionally(filter, member, cancellationToken);
						allCompleted = allCompleted && member.HasComplete(CompletionPart.All);
					}

					if (allCompleted)
					{
						_state.NotePartComplete(CompletionPart.MembersCompleted);
						Debug.Assert(_state.HasComplete(CompletionPart.MembersCompleted));
					}
					else
					{
						goto DONE;
					}

					break;
				case CompletionPart.None:
					return;
				default:
					// any other values are completion parts intended for other kinds of symbols
					_state.NotePartComplete(CompletionPart.All & ~CompletionPart.ModuleSymbolAll);
					break;
			}

			_state.SpinWaitComplete(incompletePart, cancellationToken);
		}

		DONE:
		CompletionPart allParts = (filter == null) ? CompletionPart.ModuleSymbolAll : CompletionPart.ModuleSymbolAll & ~CompletionPart.MembersCompleted;
		_state.SpinWaitComplete(allParts, cancellationToken);
	}

	internal override bool HasComplete(CompletionPart part)
	{
		return _state.HasComplete(part);
	}
}