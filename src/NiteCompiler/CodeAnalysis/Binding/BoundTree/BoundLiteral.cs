using NiteCompiler.CodeAnalysis.Binding.Pure;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundLiteral : BoundExpression
{
	public override BoundKind Kind => BoundKind.Literal;
	public ConstantValue ConstantValue { get; }
	public override TypeSymbol Type { get; }
	public override Binder.BindValueKind ValueKind => Binder.BindValueKind.RValue;
	public override Pureness Pureness => Pureness.Pure; // i assume all literals are pure

	public BoundLiteral(SyntaxNode syntax, ConstantValue constantValue, TypeSymbol type) : base(syntax)
	{
		ConstantValue = constantValue;
		Type = type;
	}

	public override void Accept(BoundVisitor visitor)
	{
		visitor.VisitLiteral(this);
	}
}