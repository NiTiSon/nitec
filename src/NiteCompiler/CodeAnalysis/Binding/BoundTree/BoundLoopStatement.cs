using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundLoopStatement : BoundStatement
{
	public BoundStatement Body { get; }

	public override BoundKind Kind => BoundKind.LoopStatement;

	public BoundLoopStatement(SyntaxNode? syntax, BoundStatement body,  bool hasErrors = false) : base(syntax, hasErrors)
	{
		Body = body;
	}

	public override void Accept(BoundVisitor visitor)
	{
		visitor.VisitLoopStatement(this);
	}
}