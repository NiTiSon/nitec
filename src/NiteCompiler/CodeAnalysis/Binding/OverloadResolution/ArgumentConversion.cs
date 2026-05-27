using NiteCompiler.CodeAnalysis.Binding.Conversions;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.CodeAnalysis.Binding.OverloadResolution;

internal readonly struct ArgumentConversion
{
	public readonly ConversionKind Kind;
	public readonly TypeSymbol FromType;
	public readonly TypeSymbol ToType;

	public ArgumentConversion(ConversionKind kind, TypeSymbol fromType, TypeSymbol toType)
	{
		Kind = kind;
		FromType = fromType;
		ToType = toType;
	}
}