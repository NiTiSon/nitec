using System.Runtime.CompilerServices;

namespace NiteCompiler;

internal static class ArrayExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNextAvilable<T>(this T[] array, int index)
		=> index < array.Length - 1;
}