using System;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.CodeAnalysis.Binding.BoundTree;

internal struct BinaryOperatorSignature : IEquatable<BinaryOperatorSignature>
{
	public static readonly BinaryOperatorSignature Error = default;

	public readonly TypeSymbol LeftType;
	public readonly TypeSymbol RightType;
	public readonly TypeSymbol ReturnType;
	public readonly FunctionSymbol? CorrespondingFunction;
	public readonly TypeSymbol? ConstrainedToTypeOpt;

	public bool Equals(BinaryOperatorSignature other)
	{
		return LeftType.Equals(other.LeftType) &&
		       RightType.Equals(other.RightType) &&
		       ReturnType.Equals(other.ReturnType) &&
		       Equals(CorrespondingFunction, other.CorrespondingFunction) &&
		       Equals(ConstrainedToTypeOpt, other.ConstrainedToTypeOpt);
	}

	public override bool Equals(object? obj)
	{
		return obj is BinaryOperatorSignature other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(LeftType, RightType, ReturnType, CorrespondingFunction, ConstrainedToTypeOpt);
	}
}