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
			SByte => BuiltinTypeSymbol.SInt8,
			Int16 => BuiltinTypeSymbol.SInt16,
			Int32 => BuiltinTypeSymbol.SInt32,
			Int64 => BuiltinTypeSymbol.SInt64,
			Byte => BuiltinTypeSymbol.UInt8,
			UInt16 => BuiltinTypeSymbol.UInt16,
			UInt32 => BuiltinTypeSymbol.UInt32,
			UInt64 => BuiltinTypeSymbol.UInt64,
			Half => BuiltinTypeSymbol.Float16,
			Single => BuiltinTypeSymbol.Float32,
			Double => BuiltinTypeSymbol.Float64,
			_ => throw new NotSupportedException()
		};

		ConstantValue = new(value);
	}

	public override TypeSymbol Type { get; }
	public override BoundConstant ConstantValue { get; }
	public override BoundKind Kind => BoundKind.LiteralExpression;
}