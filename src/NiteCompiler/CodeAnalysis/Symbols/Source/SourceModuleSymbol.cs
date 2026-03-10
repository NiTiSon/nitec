using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using NiteCompiler.CodeAnalysis.Declarations;

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

	public override ImmutableArray<Symbol> GetMembersUnordered()
	{
		Console.WriteLine("IMPLEMENT GetMembersUnordered right now!!!");
		return [];
	}

	internal override void ForceComplete(Predicate<Symbol>? filter, CancellationToken cancellationToken = default)
	{

	}

	internal override bool HasComplete(CompletionPart part)
	{
		return _state.HasComplete(part);
	}
}