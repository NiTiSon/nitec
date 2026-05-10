using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceLocalVariableSymbol : LocalVariableSymbol
{
	private readonly Binder _scopeBinder;
	private readonly TypeClauseSyntax? _typeClause;
	private readonly EqualsValueClause? _equalsValueClause;
	private readonly Binder? _initializerBinder;

	public bool IsAssignable { get; }
	public override string Name { get; }

	public override TypeSymbol Type
	{
		get
		{
			if (field == null)
			{
				Interlocked.CompareExchange(ref field, InferType(), null);
			}

			return field;
		}
	}

	private TypeSymbol InferType()
	{
    	var diagnostics = BindingDiagnosticBag.GetInstance();

	    TypeSymbol? type = null;
	    if (_equalsValueClause != null)
	    {
		    type = _initializerBinder!.BindExpression(_equalsValueClause.Expression, diagnostics, false, false).Type;
	    }

	    if (_typeClause != null)
	    {
		    // the explicit type have priority over expression implicit type
		    type = _scopeBinder.BindType(_typeClause.Type, diagnostics);
	    }

	    if (/*diagnostics.Diagnostics.HasAnyErrors ||*/type == null)
	    {
		    type = _scopeBinder.CreateErrorType();
	    }

	    // reportn't: we will report later during code binding
	    diagnostics.Free();
	    return type;
	}

	public override Symbol ContainingSymbol { get; }

	public SourceLocalVariableSymbol(Symbol containing, Binder scopeBinder,
		TypeClauseSyntax? type, EqualsValueClause? initializer, Binder? initializerBinder,
		bool isAssignable, string name,
		Location nameLocation, SyntaxReference syntaxReference)
	{
		ContainingSymbol = containing;
		_scopeBinder = scopeBinder;
		_typeClause = type;
		_equalsValueClause = initializer;
		_initializerBinder = initializerBinder;

		IsAssignable = isAssignable;
		Name = name;
	}
}