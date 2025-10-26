using System.Collections.Generic;

namespace NiteCompiler;

internal static class StackExtensions
{
	extension<T>(Stack<T> stack)
	{
		public T? PopOrDefault()
		{
			if (stack.TryPop(out T? result))
			{
				return result;
			}

			return default;
		}

		public T? PeekOrDefault()
		{
			if (stack.TryPeek(out T? result))
			{
				return result;
			}

			return default;
		}
	}
}