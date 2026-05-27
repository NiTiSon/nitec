using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Binding.Conversions;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.CodeAnalysis.Binding.OverloadResolution;

internal static class OverloadResolution
{
	public static OverloadResolutionResult Resolve(
		ImmutableArray<FunctionSymbol> candidates,
		ImmutableArray<BoundExpression> arguments)
	{
		int argCount = arguments.Length;

		var fullyApplicableFunctions = new List<(FunctionSymbol Function, ArgumentConversion[] Conversions)>();

		foreach (FunctionSymbol candidate in candidates)
		{
			int offset = candidate is ConstructorSymbol ? 1 : 0;
			int paramCount = candidate.Parameters.Length - offset;

			if (paramCount != argCount)
				continue;

			bool allImplicit = true;
			var conversions = new ArgumentConversion[argCount];

			for (int i = 0; i < argCount; i++)
			{
				TypeSymbol argType = arguments[i].Type;
				TypeSymbol paramType = candidate.Parameters[i + offset].Type;
				ConversionKind kind = TypeConversions.ClassifyConversion(argType, paramType);
				conversions[i] = new ArgumentConversion(kind, argType, paramType);

				if (kind == ConversionKind.NoConversion || !kind.IsImplicit)
					allImplicit = false;
			}

			if (allImplicit)
			{
				fullyApplicableFunctions.Add((candidate, conversions));
			}
		}

		if (fullyApplicableFunctions.Count == 0)
			return OverloadResolutionResult.Failure;

		if (fullyApplicableFunctions.Count == 1)
		{
			var (function, conversions) = fullyApplicableFunctions[0];
			return OverloadResolutionResult.Success(function, [..conversions]);
		}

		FunctionSymbol bestFunction = fullyApplicableFunctions[0].Function;
		ArgumentConversion[] bestConversions = fullyApplicableFunctions[0].Conversions;

		for (int i = 1; i < fullyApplicableFunctions.Count; i++)
		{
			FunctionSymbol candidateFunc = fullyApplicableFunctions[i].Function;
			ArgumentConversion[] candidateConvs = fullyApplicableFunctions[i].Conversions;

			int comparison = BetterFunctionMember(bestConversions, candidateConvs);

			if (comparison < 0)
			{
				bestFunction = candidateFunc;
				bestConversions = candidateConvs;
			}
			else if (comparison == 0 && bestFunction != candidateFunc)
			{
				bool distinct = false;
				for (int j = 0; j < argCount; j++)
				{
					if (bestConversions[j].Kind != candidateConvs[j].Kind ||
					    bestConversions[j].ToType != candidateConvs[j].ToType)
					{
						distinct = true;
						break;
					}
				}

				if (distinct)
					return OverloadResolutionResult.Ambiguous;
			}
		}

		return OverloadResolutionResult.Success(bestFunction, [..bestConversions]);
	}

	private static int BetterFunctionMember(ArgumentConversion[] convsA, ArgumentConversion[] convsB)
	{
		bool aHasBetter = false;
		bool bHasBetter = false;

		for (int i = 0; i < convsA.Length; i++)
		{
			int cmp = BetterConversion(convsA[i], convsB[i]);
			if (cmp > 0)
				aHasBetter = true;
			else if (cmp < 0)
				bHasBetter = true;
		}

		if (aHasBetter && !bHasBetter) return 1;
		if (bHasBetter && !aHasBetter) return -1;
		return 0;
	}

	private static int BetterConversion(ArgumentConversion a, ArgumentConversion b)
	{
		if (a.Kind == ConversionKind.Identity && b.Kind != ConversionKind.Identity)
			return 1;
		if (b.Kind == ConversionKind.Identity && a.Kind != ConversionKind.Identity)
			return -1;
		if (a.Kind == ConversionKind.Identity && b.Kind == ConversionKind.Identity)
			return 0;

		bool impA = a.Kind.IsImplicit;
		bool impB = b.Kind.IsImplicit;

		if (impA && !impB) return 1;
		if (impB && !impA) return -1;

		if (!impA && !impB)
			return 0;

		if (a.Kind == b.Kind && a.FromType == b.FromType)
		{
			int fromRank = GetNumericTypeRank(a.FromType.SpecialType);
			int rankA = GetNumericTypeRank(a.ToType.SpecialType);
			int rankB = GetNumericTypeRank(b.ToType.SpecialType);

			if (fromRank >= 0 && rankA >= 0 && rankB >= 0)
			{
				int distA = Math.Abs(rankA - fromRank);
				int distB = Math.Abs(rankB - fromRank);
				if (distA < distB) return 1;
				if (distB < distA) return -1;
			}
		}

		if (a.Kind == ConversionKind.ImplicitNumeric &&
		    b.Kind == ConversionKind.ImplicitSignedIntegerToFloat)
			return 1;
		if (b.Kind == ConversionKind.ImplicitNumeric &&
		    a.Kind == ConversionKind.ImplicitSignedIntegerToFloat)
			return -1;

		if (a.Kind == ConversionKind.ImplicitFloatExtension &&
		    b.Kind == ConversionKind.ImplicitSignedIntegerToFloat)
			return 1;
		if (b.Kind == ConversionKind.ImplicitFloatExtension &&
		    a.Kind == ConversionKind.ImplicitSignedIntegerToFloat)
			return -1;

		return 0;
	}

	private static int GetNumericTypeRank(SpecialType type)
	{
		return type switch
		{
			SpecialType.StdNumericsSInt8 => 0,
			SpecialType.StdNumericsSInt16 => 1,
			SpecialType.StdNumericsSInt32 => 2,
			SpecialType.StdNumericsSInt64 => 3,
			SpecialType.StdNumericsUInt8 => 4,
			SpecialType.StdNumericsUInt16 => 5,
			SpecialType.StdNumericsUInt32 => 6,
			SpecialType.StdNumericsUInt64 => 7,
			SpecialType.StdNumericsFloat16 => 8,
			SpecialType.StdNumericsFloat32 => 9,
			SpecialType.StdNumericsFloat64 => 10,
			_ => -1,
		};
	}
}
