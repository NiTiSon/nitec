namespace NiteCompiler.CodeAnalysis.Binding.OverloadResolution;

internal enum OverloadResolutionStatus : byte
{
	Success,
	Failure,
	Ambiguous,
}