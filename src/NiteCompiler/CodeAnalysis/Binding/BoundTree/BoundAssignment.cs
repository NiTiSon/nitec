using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundAssignment : BoundExpression
{
	public BoundExpression Left { get; }
	public BoundExpression Right { get; }

	public override BoundKind Kind => BoundKind.AssignmentExpression;
	public override TypeSymbol Type => Left.Type;
	public override Binder.BindValueKind ValueKind => Binder.BindValueKind.RValue;

	public BoundAssignment(SyntaxNode syntax, BoundExpression left, BoundExpression right) : base(syntax)
	{
		Left = left;
		Right = right;
	}

	public override void Accept(BoundVisitor visitor)
	{
		visitor.VisitAssignment(this);
	}
}