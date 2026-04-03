using System;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.CodeAnalysis.Binding.Operators;

internal struct UnaryOperatorSignature : IEquatable<UnaryOperatorSignature>
{
	public static readonly UnaryOperatorSignature Error = default;

	public readonly TypeSymbol InputType;
	public readonly TypeSymbol ReturnType;
	public readonly FunctionSymbol? CorrespondingFunction;
	public readonly UnaryOperatorKind Kind;

	public UnaryOperatorSignature(UnaryOperatorKind kind, TypeSymbol inputType, TypeSymbol returnType)
	{
		Kind = kind;
		InputType = inputType;
		ReturnType = returnType;
		CorrespondingFunction = null;
	}

	public UnaryOperatorSignature(UnaryOperatorKind kind, TypeSymbol inputType, TypeSymbol returnType, FunctionSymbol function)
	{
		Kind = kind;
		InputType = inputType;
		ReturnType = returnType;
		CorrespondingFunction = function;
	}


	public bool Equals(UnaryOperatorSignature other)
	{
		return Kind.Equals(other.Kind) &&
		       InputType.Equals(other.InputType) &&
		       ReturnType.Equals(other.ReturnType) &&
		       Equals(CorrespondingFunction, other.CorrespondingFunction);
	}

	public override bool Equals(object? obj)
	{
		return obj is UnaryOperatorSignature other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Kind, InputType, ReturnType, CorrespondingFunction);
	}
}