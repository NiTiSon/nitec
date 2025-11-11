using System;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundLiteralExpression : BoundExpression
{
	public BoundLiteralExpression(SyntaxNode syntax, object value) : base(syntax)
	{
		Type = value switch
		{
			_ => null!,
		};

		ConstantValue = new(value);
	}

	public override TypeSymbol Type { get; }
	public override BoundConstant ConstantValue { get; }
	public override BoundKind Kind => BoundKind.LiteralExpression;
}