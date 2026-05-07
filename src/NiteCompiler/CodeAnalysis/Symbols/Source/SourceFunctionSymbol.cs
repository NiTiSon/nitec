using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceFunctionSymbol : FunctionSymbol
{
	public override Symbol ContainingSymbol { get; }
	public FunctionDeclarationSyntax Syntax { get; }
	public override string Name => Syntax.Name.GetName();

	private CompletionPart _state;
	public SourceFunctionSymbol(Symbol containingSymbol, FunctionDeclarationSyntax syntax)
	{
		Debug.Assert(containingSymbol != null);

		ContainingSymbol = containingSymbol;
		Syntax = syntax;
	}

	public override TypeSymbol ReturnType
	{
		get
		{
			if (field == null)
			{
				Interlocked.CompareExchange(ref field, MakeReturnType(), null);
			}

			return field;
		}
	}

	private TypeSymbol MakeReturnType()
	{
		BindingDiagnosticBag diagnostics = BindingDiagnosticBag.GetInstance();

		BinderFactory factory = DeclaringCompilation!.GetBinderFactory(Syntax.Tree);

		if (Syntax.ReturnTypeClause == null) // void
		{
			TypeSymbol @void = (TypeSymbol)factory.GetBinder(Syntax).BindVoidType();
			return @void;
		}
		Binder withGenericsBinder = factory.GetBinder(Syntax.ReturnTypeClause);
		TypeSymbol result = withGenericsBinder.BindType(Syntax.ReturnTypeClause.Type, diagnostics);

		diagnostics.Free();
		return result;
	}

	public override ImmutableArray<LifetimeParameterSymbol> Lifetimes
	{
		get
		{
			if (field.IsDefault)
			{
				ImmutableInterlocked.InterlockedCompareExchange(ref field, MakeLifetimeParameters(), default);
			}

			return field;
		}
	}

	private ImmutableArray<LifetimeParameterSymbol> MakeLifetimeParameters()
	{
		if (Syntax.Name is not GenericNameSyntax genericNameSyntax)
		{
			return [];
		}

		var builder = ArrayBuilder<LifetimeParameterSymbol>.GetInstance();

		int ordinal = 0;
		foreach (var parameterSyntax in genericNameSyntax.GenericParameterList.Parameters)
		{
			if (parameterSyntax is not LifetimeSyntax lifetime) continue;

			var symbol = new SourceLifetimeParameterSymbol(this, lifetime.Identifier, ordinal);
			builder.Add(symbol);

			ordinal++;
		}

		return builder.ToImmutable();
	}

	public override ImmutableArray<ParameterSymbol> Parameters
	{
		get
		{
			if (field.IsDefault)
			{
				ImmutableInterlocked.InterlockedCompareExchange(ref field, MakeParameters(), default);
			}

			return field;
		}
	}

	private ImmutableArray<ParameterSymbol> MakeParameters()
	{
		BindingDiagnosticBag diagnostics = BindingDiagnosticBag.GetInstance();
		ImmutableArray<ParameterSymbol>.Builder builder = ImmutableArray.CreateBuilder<ParameterSymbol>();
		BinderFactory factory = DeclaringCompilation!.GetBinderFactory(Syntax.Tree);
		Binder withGenericsBinder = factory.GetBinder(Syntax.ParameterList);
		int ordinal = 0;
		foreach (var parameter in Syntax.ParameterList.Parameters)
		{
			TypeSymbol type = withGenericsBinder.BindType(parameter.TypeClause.Type, diagnostics);
			var parameterSymbol = SourceParameterSymbol.Create(withGenericsBinder, this, type, parameter, ordinal, diagnostics);
			builder.Add(parameterSymbol);
			ordinal++;
		}

		diagnostics.Free();
		return builder.ToImmutable();
	}

	private FunctionBodySyntax GetInFunctionSyntaxNode()
	{
		return Syntax.Body;
	}

	public Binder? TryGetInFunctionBinder(BinderFactory? binderFactory = null)
	{
		SyntaxNode inNode = GetInFunctionSyntaxNode();

		Binder result = (binderFactory ?? DeclaringCompilation!.GetBinderFactory(inNode.Tree)).GetBinder(inNode);
#if DEBUG
		Binder? current = result;
		do
		{
			if (current is InFunctionBinder)
			{
				break;
			}

			current = current.Parent;
		}
		while (current != null);

		Debug.Assert(current is InFunctionBinder);
#endif
		return result;
	}

	public Binder? TryGetBodyBinder()
	{
		Binder? inFunctionBinder = TryGetInFunctionBinder();
		FunctionBodySyntax body = GetInFunctionSyntaxNode();
		SyntaxNode? syntax = null;
		if (body is BlockFunctionBodySyntax blockBody)
		{
			syntax = blockBody.Block;
		}

		return inFunctionBinder == null
			? null
			: (syntax == null ? inFunctionBinder : new ExecutableCodeBinder(syntax, this, inFunctionBinder));
	}

	public bool TryBindBody(
		bool lower,
		[NotNullWhen(true)] out Binder? binder,
		[NotNullWhen(true)] out BoundFunctionBody? body,
		BindingDiagnosticBag diagnostics)
	{
		binder = TryGetBodyBinder();
		body = null;

		if (binder == null)
		{
			return false;
		}

		body = (BoundFunctionBody)binder.BindFunctionBody(Syntax, diagnostics);
		Debug.Assert(body != null);
		return true;
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
				case CompletionPart.None:
					return;
				case CompletionPart.Type:
					_ = ReturnType;
					_state.NotePartComplete(CompletionPart.Type);
					break;
				case CompletionPart.LifetimeParameters:
					_ = Lifetimes;
					_state.NotePartComplete(CompletionPart.LifetimeParameters);
					break;
				case CompletionPart.GenericParameters:
					// TODO[generics]
					_state.NotePartComplete(CompletionPart.GenericParameters);
					break;
				case CompletionPart.Parameters:
					foreach (var parameter in Parameters)
					{
						parameter.ForceComplete(filter: null, cancellationToken);
					}
					_state.NotePartComplete(CompletionPart.Parameters);
					break;
				default:
					_state.NotePartComplete(CompletionPart.All & ~CompletionPart.FunctionSymbolAll);
					break;
			}

			_state.SpinWaitComplete(incompletePart, cancellationToken);
		}
	}
}