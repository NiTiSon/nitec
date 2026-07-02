using Microsoft.CodeAnalysis;

namespace NiteCompilerUtils.SourceGenerators;

[Generator]
public sealed class TokenGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
	}
}