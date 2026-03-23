using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding.BoundTree;

internal sealed class BoundLiteral : BoundExpression
{
	public override BoundKind Kind => BoundKind.Literal;
	public ConstantValue ConstantValue { get; }
	public override TypeSymbol Type { get; }

	public BoundLiteral(SyntaxNode syntax, ConstantValue constantValue, TypeSymbol type) : base(syntax)
	{
		ConstantValue = constantValue;
		Type = type;
	}

	public override Binder.BindValueKind ValueKind => Binder.BindValueKind.RValue;
}