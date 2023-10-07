namespace NiteCompiler;

internal static class ArrayExtensions
{
	public static bool IsNextAvilable<T>(this T[] array, int index)
		=> index < array.Length - 1;
}