using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundEmptyStatement : BoundStatement
{
	public BoundEmptyStatement(EmptyStatementSyntax syntax) : base(syntax, hasErrors: false)
	{
	}

	public override BoundKind Kind => BoundKind.EmptyStatement;
	public override void Accept(BoundVisitor visitor)
	{
		visitor.VisitEmptyStatement(this);
	}
}