using System.Collections.Immutable;
using System.Threading;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Binding.Operators;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.Compilation;

internal sealed class BuiltInOperators
{
	private readonly NiteCompilation _compilation;

	private ImmutableArray<UnaryOperatorSignature>[]? _lateinitUnaryOperators;
	private ImmutableArray<BinaryOperatorSignature>[]? _lateinitBinaryOperators;
	public BuiltInOperators(NiteCompilation compilation)
	{
		_compilation = compilation;
	}

	private TypeSymbol GetSpecialType(SpecialType type)
	{
		return _compilation.GetSpecialType(type)!;
	}

	public void GetOperators(UnaryOperatorKind kind, ArrayBuilder<UnaryOperatorSignature> operators)
	{
		if (_lateinitUnaryOperators == null)
		{
			TypeSymbol boolean = GetSpecialType(SpecialType.StdBoolean);
			ImmutableArray<UnaryOperatorSignature>[] unaryOperators =
			[
				GetArithmeticOperatorSignatures(UnaryOperatorKind.Plus),
				GetArithmeticOperatorSignatures(UnaryOperatorKind.Negate),
				GetBitwiseOperatorSignature(UnaryOperatorKind.BitwiseNot),
				[ // !not
					new UnaryOperatorSignature(UnaryOperatorKind.LogicalNot, boolean, boolean)
				],
				[] // ^circumflex
			];

			Interlocked.CompareExchange(ref _lateinitUnaryOperators, unaryOperators, null);
		}

		operators.AddRange(_lateinitUnaryOperators[kind.ToIndex()]);
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
				GetBitwiseOperatorSignatures(BinaryOperatorKind.And),
				GetBitwiseOperatorSignatures(BinaryOperatorKind.Xor),
				GetBitwiseOperatorSignatures(BinaryOperatorKind.Or),
				[] // There are no built-in binary tilde operators
			];

			Interlocked.CompareExchange(ref _lateinitBinaryOperators, binaryOperators, null);
		}

		operators.AddRange(_lateinitBinaryOperators[kind.ToIndex()]);
	}

	private ImmutableArray<UnaryOperatorSignature> GetArithmeticOperatorSignatures(UnaryOperatorKind kind)
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

	private ImmutableArray<UnaryOperatorSignature> GetBitwiseOperatorSignature(UnaryOperatorKind kind)
	{
		return GetArithmeticOperatorSignatures(kind);
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
	private UnaryOperatorSignature GetArithmeticOperatorSignature(UnaryOperatorKind kind, TypeSymbol type)
	{
		return new UnaryOperatorSignature(kind, type, type);
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

	private ImmutableArray<BinaryOperatorSignature> GetBitwiseOperatorSignatures(BinaryOperatorKind kind)
	{
		TypeSymbol boolean = GetSpecialType(SpecialType.StdBoolean);
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
			GetArithmeticOperatorSignature(kind, GetSpecialType(SpecialType.StdNumericsFloat64)),
			new BinaryOperatorSignature(kind, boolean, boolean, boolean)
		];
	}
}