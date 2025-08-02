using System.Runtime.CompilerServices;

namespace NiTiS.Compiler.NiteCode.CodeAnalysis;

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