using System;
using System.Diagnostics;
using System.Threading;

namespace NiteCompiler.CodeAnalysis.Symbols;

[Flags]
internal enum CompletionPart
{
	None = 0,
	Attributes = 1 << 0,
	Type = 1 << 1,
	LifetimeParameters = 1 << 2,
	GenericParameters = 1 << 3,
	MembersCompleted = 1 << 4,
	All = (1 << 5) - 1,

	// Modules
	NameToMembersMap = Type,
	// + MembersCompleted

	// Functions
	// + Type
	// + LifetimeParameters
	// + GenericParameters
	Parameters = MembersCompleted,

	ModuleSymbolAll = NameToMembersMap | MembersCompleted,
	LibrarySymbolAll = MembersCompleted,
	TypeSymbolAll = MembersCompleted | LifetimeParameters | GenericParameters,
	FunctionSymbolAll = Type | LifetimeParameters | GenericParameters | Parameters,
	ParameterSymbolAll = Type,
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

		private int IncompleteParts => ~(int)self & (int)CompletionPart.All;

		public CompletionPart NextIncompletePart
		{
			get
			{
				int incomplete = self.IncompleteParts;
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