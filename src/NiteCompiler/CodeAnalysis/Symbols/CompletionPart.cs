using System;
using System.Threading;

namespace NiteCompiler.CodeAnalysis.Symbols;

[Flags]
internal enum CompletionPart
{
	None = 0,

	// Functions completion parts:
	ReturnType = 1 << 1,

	All = (1 << 2) - 1,
}

internal static class CompletionPartExtensions
{
	extension(ref CompletionPart self)
	{
		public bool HasComplete(CompletionPart part)
		{
			return self.HasFlag(part);
		}

		public bool NotePartComplete(CompletionPart part)
		{
			CompletionPart oldState, newState;
			do
			{
				oldState = self;
				newState = oldState | part;
				if (newState == oldState)
				{
					return false;
				}
			}
			while (Interlocked.CompareExchange(ref self, newState, oldState) != oldState);
			return true;
		}

		public void SpinWaitComplete(CompletionPart part, CancellationToken cancellationToken = default)
		{
			if (self.HasComplete(part))
			{
				return;
			}

			SpinWait spinWait = new();
			while (!self.HasComplete(part))
			{
				cancellationToken.ThrowIfCancellationRequested();
				spinWait.SpinOnce();
			}
		}
	}
}