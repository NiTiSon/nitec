using System.Runtime.InteropServices;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundReturnStatement : BoundStatement
{
	public BoundExpression? Expression { get; }
	public override BoundKind Kind => BoundKind.ReturnStatement;

	public BoundReturnStatement(SyntaxNode syntax, [Optional] BoundExpression? expression) : base(syntax)
	{
		Expression = expression;
	}
}