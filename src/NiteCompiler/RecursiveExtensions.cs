using System;

namespace NiteCompiler;

internal static class RecursiveExtensions
{
	extension<T>(T self) where T : class
	{
		public T GetTopMostRecursively(Func<T, T?> accessor)
		{
			T last = self;
			do
			{
				T? n = accessor(last);
				if (n is null) break;

				last = n;
			} while (true);

			return last;
		}
	}
}