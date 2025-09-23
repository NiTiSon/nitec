using System.Runtime.CompilerServices;

namespace NiteCompiler.CodeAnalysis;

public static class StackGuard
{
	public const int MaxStackSize = 20;

	public static void EnsureSufficientStackSize(int depth)
	{
		if (depth > MaxStackSize)
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
		}
	}
}