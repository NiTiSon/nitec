using NiteCompiler.CodeAnalysis.Binding.Conversions;
using NiteCompiler.CodeAnalysis.Binding.Pure;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundConversion : BoundExpression
{
	public BoundConversion(Syntax.SyntaxNode syntax, BoundExpression operand, ConversionKind conversionKind, TypeSymbol type, bool hasErrors = false)
		: base(syntax, hasErrors)
	{
		Operand = operand;
		ConversionKind = conversionKind;
		Type = type;
	}

	public override BoundKind Kind => BoundKind.Conversion;
	public override TypeSymbol Type { get; }
	public override Pureness Pureness => Operand.Pureness;
	public override Binder.BindValueKind ValueKind => Binder.BindValueKind.RValue;

	public BoundExpression Operand { get; }
	public ConversionKind ConversionKind { get; }

	public override void Accept(BoundVisitor visitor)
	{
		visitor.VisitConversion(this);
	}
}
