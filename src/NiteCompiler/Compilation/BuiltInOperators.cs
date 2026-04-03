using System.Collections.Immutable;
using System.Threading;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.Compilation;

internal sealed class BuiltInOperators
{
	private readonly NiteCompilation _compilation;

	private ImmutableArray<BinaryOperatorSignature>[]? _lateinitBinaryOperators;
	public BuiltInOperators(NiteCompilation compilation)
	{
		_compilation = compilation;
	}

	private TypeSymbol GetSpecialType(SpecialType type)
	{
		return _compilation.GetSpecialType(type)!;
	}

	public void GetOperators(BinaryOperatorKind kind, ArrayBuilder<BinaryOperatorSignature> operators)
	{
		if (_lateinitBinaryOperators == null)
		{
			ImmutableArray<BinaryOperatorSignature>[] binaryOperators =
			[
				GetArithmeticOperatorSignatures(BinaryOperatorKind.Addition),
				GetArithmeticOperatorSignatures(BinaryOperatorKind.Subtraction),
				GetArithmeticOperatorSignatures(BinaryOperatorKind.Multiplication),
				GetArithmeticOperatorSignatures(BinaryOperatorKind.Division),
				GetArithmeticOperatorSignatures(BinaryOperatorKind.Modulo),
				[], // SAL
				[], // SAR
				[], // SHR
				GetEqualityOperatorSignatures(BinaryOperatorKind.Equal),
				GetEqualityOperatorSignatures(BinaryOperatorKind.NotEqual),
				GetComparingOperatorSignatures(BinaryOperatorKind.Greater),
				GetComparingOperatorSignatures(BinaryOperatorKind.Less),
				GetComparingOperatorSignatures(BinaryOperatorKind.GreaterOrEqual),
				GetComparingOperatorSignatures(BinaryOperatorKind.LessOrEqual),
				GetLogicalOperatorSignatures(BinaryOperatorKind.And),
				GetLogicalOperatorSignatures(BinaryOperatorKind.Or),
				GetLogicalOperatorSignatures(BinaryOperatorKind.Xor),
				[] // There are no built-in binary tilde operators
			];

			Interlocked.CompareExchange(ref _lateinitBinaryOperators, binaryOperators, null);
		}

		operators.AddRange(_lateinitBinaryOperators[(int)kind]);
	}

	private ImmutableArray<BinaryOperatorSignature> GetArithmeticOperatorSignatures(BinaryOperatorKind kind)
	{
		return
		[
			GetArithmeticOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsSInt8)),
			GetArithmeticOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsSInt16)),
			GetArithmeticOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsSInt32)),
			GetArithmeticOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsSInt64)),
			GetArithmeticOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsSNativeInt)),
			GetArithmeticOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsUInt8)),
			GetArithmeticOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsUInt16)),
			GetArithmeticOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsUInt32)),
			GetArithmeticOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsUInt64)),
			GetArithmeticOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsUNativeInt)),
			GetArithmeticOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsFloat16)),
			GetArithmeticOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsFloat32)),
			GetArithmeticOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsFloat64))
		];
	}

	private BinaryOperatorSignature GetArithmeticOperatorSignature(BinaryOperatorKind kind, TypeSymbol type)
	{
		return new BinaryOperatorSignature(kind, type, type, type);
	}

	private ImmutableArray<BinaryOperatorSignature> GetEqualityOperatorSignatures(BinaryOperatorKind kind)
	{
		return
		[
			GetComparingOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsSInt8)),
			GetComparingOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsSInt16)),
			GetComparingOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsSInt32)),
			GetComparingOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsSInt64)),
			GetComparingOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsSNativeInt)),
			GetComparingOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsUInt8)),
			GetComparingOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsUInt16)),
			GetComparingOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsUInt32)),
			GetComparingOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsUInt64)),
			GetComparingOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsUNativeInt)),
			GetComparingOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsFloat16)),
			GetComparingOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsFloat32)),
			GetComparingOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsFloat64)),
			GetComparingOperatorSignature(kind, GetSpecialType(SpecialType.StdBoolean)), // == and != also valid for booleans
		];
	}

	private ImmutableArray<BinaryOperatorSignature> GetComparingOperatorSignatures(BinaryOperatorKind kind)
	{
		return
		[
			GetComparingOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsSInt8)),
			GetComparingOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsSInt16)),
			GetComparingOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsSInt32)),
			GetComparingOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsSInt64)),
			GetComparingOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsSNativeInt)),
			GetComparingOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsUInt8)),
			GetComparingOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsUInt16)),
			GetComparingOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsUInt32)),
			GetComparingOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsUInt64)),
			GetComparingOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsUNativeInt)),
			GetComparingOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsFloat16)),
			GetComparingOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsFloat32)),
			GetComparingOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsFloat64)),
		];
	}

	private BinaryOperatorSignature GetComparingOperatorSignature(BinaryOperatorKind kind, TypeSymbol type)
	{
		return new BinaryOperatorSignature(kind, type, type, GetSpecialType(SpecialType.StdBoolean));
	}

	private ImmutableArray<BinaryOperatorSignature> GetLogicalOperatorSignatures(BinaryOperatorKind kind)
	{
		TypeSymbol boolean = GetSpecialType(SpecialType.StdBoolean);
		return
		[
			new BinaryOperatorSignature(kind, boolean, boolean, boolean)
		];
	}
}