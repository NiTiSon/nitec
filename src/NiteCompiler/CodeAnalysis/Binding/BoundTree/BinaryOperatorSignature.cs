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
	public readonly BinaryOperatorKind Kind;

	public BinaryOperatorSignature(BinaryOperatorKind kind, TypeSymbol leftType, TypeSymbol rightType, TypeSymbol returnType)
	{
		Kind = kind;
		LeftType = leftType;
		RightType = rightType;
		ReturnType = returnType;
		CorrespondingFunction = null;
	}

	public BinaryOperatorSignature(BinaryOperatorKind kind, TypeSymbol leftType, TypeSymbol rightType, TypeSymbol returnType, FunctionSymbol function)
	{
		Kind = kind;
		LeftType = leftType;
		RightType = rightType;
		ReturnType = returnType;
		CorrespondingFunction = function;
	}


	public bool Equals(BinaryOperatorSignature other)
	{
		return Kind.Equals(other.Kind) &&
			   LeftType.Equals(other.LeftType) &&
		       RightType.Equals(other.RightType) &&
		       ReturnType.Equals(other.ReturnType) &&
		       Equals(CorrespondingFunction, other.CorrespondingFunction);
	}

	public override bool Equals(object? obj)
	{
		return obj is BinaryOperatorSignature other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Kind, LeftType, RightType, ReturnType, CorrespondingFunction);
	}
}