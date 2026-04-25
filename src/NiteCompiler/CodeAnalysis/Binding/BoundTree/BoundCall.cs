using System.Collections.Immutable;
using System.Linq;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundCall : BoundExpression
{
	public FunctionSymbol Function { get; }
	public ImmutableArray<BoundExpression> Arguments { get; }

	public BoundCall(InvocationExpressionSyntax syntax, FunctionSymbol function,
		ImmutableArray<BoundExpression> arguments, bool hasErrors = false)
		: base(syntax, hasErrors || function.IsErrorSymbol || arguments.Any(t => t.HasErrors))
	{
		Function = function;
		Arguments = arguments;
	}

	public override BoundKind Kind => BoundKind.InvocationExpression;
	public override TypeSymbol Type => Function.ReturnType;
	public override Binder.BindValueKind ValueKind => Binder.BindValueKind.RValue;

	public override void Accept(BoundVisitor visitor) =>visitor.VisitCall(this);
}