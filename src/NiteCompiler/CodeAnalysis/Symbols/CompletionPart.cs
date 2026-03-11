using System;
using System.Diagnostics;
using System.Threading;

namespace NiteCompiler.CodeAnalysis.Symbols;

[Flags]
internal enum CompletionPart
{
	None = 0,
	Attributes = 1 << 0,
	MembersCompleted = 1 << 10,
	All = (1 << 11) - 1,

	// Modules
	NameToMembersMap = 1 << 9,
	ModuleSymbolAll = NameToMembersMap | MembersCompleted,
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

		public CompletionPart NextIncompletePart
		{
			get
			{
				int incomplete = (int)self;
				int next = incomplete & ~(incomplete - 1);
				Debug.Assert(CompletionPart.HasAtMostOneBitSet(next), "ForceComplete won't handle the result correctly if more than one bit is set.");
				return (CompletionPart)next;
			}
		}

		private static bool HasAtMostOneBitSet(int bits)
		{
			return (bits & (bits - 1)) == 0;
		}
	}
}