using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.CodeAnalysis.Binding.OverloadResolution;

internal readonly struct OverloadResolutionResult
{
	public readonly FunctionSymbol? BestFunction;
	public readonly ImmutableArray<ArgumentConversion> Conversions;
	public readonly OverloadResolutionStatus Status;

	private OverloadResolutionResult(FunctionSymbol? function, ImmutableArray<ArgumentConversion> conversions,
		OverloadResolutionStatus status)
	{
		BestFunction = function;
		Conversions = conversions;
		Status = status;
	}

	public static OverloadResolutionResult Success(FunctionSymbol function, ImmutableArray<ArgumentConversion> conversions)
		=> new(function, conversions, OverloadResolutionStatus.Success);

	public static OverloadResolutionResult Failure => new(null, default, OverloadResolutionStatus.Failure);

	public static OverloadResolutionResult Ambiguous => new(null, default, OverloadResolutionStatus.Ambiguous);
}