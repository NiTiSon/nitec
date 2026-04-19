using System;
using System.Collections.Immutable;
using System.Diagnostics;
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

	public override void Accept(SymbolVisitor visitor)
	{
		visitor.VisitFunction(this);
	}

	public override TResult? Accept<TResult>(SymbolVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitFunction(this);
	}

	public override TResult? Accept<TResult, TArgument>(SymbolVisitor<TResult, TArgument> visitor, TArgument arg) where TResult : default
	{
		return visitor.VisitFunction(this, arg);
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

		if (Syntax.TypeClause == null) // void
		{
			TypeSymbol @void = (TypeSymbol)factory.GetBinder(Syntax).BindVoidType();
			return @void;
		}
		Binder withGenericsBinder = factory.GetBinder(Syntax.TypeClause);
		TypeSymbol result = withGenericsBinder.BindType(Syntax.TypeClause.Type, diagnostics);

		diagnostics.Free();
		return result;
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

	public ExecutableCodeBinder? TryGetBodyBinder()
	{
		Binder? inFunctionBinder = TryGetInFunctionBinder();
		FunctionBodySyntax body = GetInFunctionSyntaxNode();
		SyntaxNode? syntax = null;
		if (body is BlockFunctionBodySyntax blockBody)
		{
			syntax = blockBody.Block;
		}

		Debug.Assert(syntax != null);
		return inFunctionBinder == null ? null : new ExecutableCodeBinder(syntax, this, inFunctionBinder);
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