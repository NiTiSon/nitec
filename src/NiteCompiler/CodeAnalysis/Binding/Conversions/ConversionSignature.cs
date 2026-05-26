using System;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.CodeAnalysis.Binding.Conversions;

internal readonly struct ConversionSignature : IEquatable<ConversionSignature>
{
	public static readonly ConversionSignature Error = default;

	public readonly TypeSymbol InputType;
	public readonly TypeSymbol ResultType;
	public readonly FunctionSymbol? CorrespondingFunction;
	public readonly ConversionKind Kind;

	public ConversionSignature(ConversionKind kind, TypeSymbol inputType, TypeSymbol resultType)
	{
		Kind = kind;
		InputType = inputType;
		ResultType = resultType;
		CorrespondingFunction = null;
	}

	public bool Equals(ConversionSignature other)
	{
		return InputType == other.InputType &&
		       ResultType == other.ResultType &&
		       CorrespondingFunction == other.CorrespondingFunction &&
		       Kind == other.Kind;
	}
}