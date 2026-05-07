using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using LLVMSharp;
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

	public override ImmutableArray<LifetimeSymbol> Lifetimes
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

	public override ImmutableArray<LifetimeConstraint> LifetimeConstraints
	{
		get
		{
			if (field.IsDefault)
			{
				ImmutableInterlocked.InterlockedCompareExchange(ref field, MakeLifetimeConstraints(), default);
			}

			return field;
		}
	}

	private ImmutableArray<LifetimeSymbol> MakeLifetimeParameters()
	{
		if (Syntax.Name is not GenericNameSyntax genericNameSyntax)
		{
			return [];
		}

		var builder = ArrayBuilder<LifetimeSymbol>.GetInstance();

		var diagnostics = BindingDiagnosticBag.GetInstance();

		HashSet<string> names = [];

		int ordinal = 0;
		foreach (LifetimeOrGenericParameterSyntax parameterSyntax in genericNameSyntax.GenericParameterList.Parameters)
		{
			if (parameterSyntax is not LifetimeSyntax lifetimeSyntax)
			{
				continue;
			}

			string name = lifetimeSyntax.Identifier;

			if (!names.Add(name))
			{
				diagnostics.Diagnostics.ReportDuplicateLifetimeParameter(lifetimeSyntax.Location);

				continue;
			}

			var symbol = new SourceLifetimeSymbol(
				this,
				lifetimeSyntax.Identifier,
				ordinal);

			builder.Add(symbol);

			ordinal++;
		}

		AddDeclarationDiagnostics(diagnostics);
		diagnostics.Free();

		return builder.ToImmutable();
	}

	private ImmutableArray<LifetimeConstraint> MakeLifetimeConstraints()
	{
		// need WithLifetimesBinder
		throw new NotImplementedException();
		// if (Lifetimes.Length == 0 || Syntax.Name is not GenericNameSyntax nameSyntax || Syntax.ConstraintClauses == null)
		// {
		// 	return ImmutableArray<LifetimeConstraint>.Empty;
		// }
		//
		// BindingDiagnosticBag diagnostics = BindingDiagnosticBag.GetInstance();
		// HashSet<LifetimeSyntax> lifetimes = [];
		//
		// Dictionary<string, LifetimeSymbol> map = Lifetimes.ToDictionary(t => t.Name);
		// ArrayBuilder<LifetimeConstraint> builder = ArrayBuilder<LifetimeConstraint>.GetInstance();
		// foreach (LifetimeOrGenericConstraintClauseSyntax constraintSyntax in Syntax.ConstraintClauses)
		// {
		// 	if (constraintSyntax is not LifetimeConstraintClauseSyntax lifetimeConstraintClause)
		// 	{
		// 		continue;
		// 	}
		//
		// 	string longerLifetimeName = lifetimeConstraintClause.Lifetime.Identifier;
		// 	if (!map.TryGetValue(longerLifetimeName, out LifetimeSymbol? lifetime))
		// 	{
		// 		diagnostics.Diagnostics.ReportUnresolvedSymbol(lifetimeConstraintClause.Lifetime.Location);
		// 		continue; // we don't want to overwhelm user with diagnostics, so just ignore constraints in such scenario
		// 	}
		//
		// 	foreach (ConstraintSyntax constraint in lifetimeConstraintClause.Constraints)
		// 	{
		// 		if (constraint is not LifetimeConstraintSyntax lifetimeConstraint)
		// 		{
		// 			// TODO: diagnostic
		// 		}
		// 	}
		//
		// 	if (!map.TryGetValue(longerLifetimeName, out LifetimeSymbol? longer))
		// 	{
		// 		diagnostics.Diagnostics.ReportUndefinedLifetime(
		// 			lifetimeConstraint.LifetimeIdentifier.Location,
		// 			longerLifetimeName);
		//
		// 		continue;
		// 	}
		//
		// 	if (!map.TryGetValue(shorterName, out LifetimeSymbol? shorter))
		// 	{
		// 		diagnostics.Diagnostics.ReportUndefinedLifetime(
		// 			lifetimeConstraint.TargetLifetime.Location,
		// 			shorterName);
		//
		// 		continue;
		// 	}
		//
		// 	builder.Add(new LifetimeConstraint(
		// 		longer,
		// 		shorter));
		// }
		//
		// AddDeclarationDiagnostics(diagnostics);
		// diagnostics.Free();
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