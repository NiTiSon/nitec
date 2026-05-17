using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using NiteCompiler.CodeAnalysis.Binding.Pure;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundCall : BoundExpression
{
	public FunctionSymbol Function { get; }
	public ImmutableArray<BoundExpression> Arguments { get; }

	public override Pureness Pureness
	{
		get
		{
			Pureness pureness = Pureness.Pure;

			pureness += Function.Pureness;
			foreach (var arg in Arguments)
			{
				pureness += arg.Pureness;
			}

			return pureness;
		}
	}

public BoundCall(InvocationExpressionSyntax syntax, FunctionSymbol function,
	ImmutableArray<BoundExpression> arguments, TypeSymbol? typeOverride = null, bool hasErrors = false)
	: base(syntax, hasErrors || function.IsErrorSymbol || arguments.Any(t => t.HasErrors))
{
	Function = function;
	Arguments = arguments;
	_typeOverride = typeOverride;
}

private readonly TypeSymbol? _typeOverride;

public override BoundKind Kind => BoundKind.InvocationExpression;
public override TypeSymbol Type => _typeOverride ?? Function.ReturnType;
	public override Binder.BindValueKind ValueKind => Binder.BindValueKind.RValue;

	public override void Accept(BoundVisitor visitor) =>visitor.VisitCall(this);
}