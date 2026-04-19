using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace NiteCompiler;

internal static class ParallelExtensions
{
	private static readonly ParallelOptions DefaultParallelOptions = new() { MaxDegreeOfParallelism = Environment.ProcessorCount };

	extension(Parallel)
	{
		/// <inheritdoc cref="Parallel.For(int, int, Action{int})"/>
		public static ParallelLoopResult For(int fromInclusive, int toExclusive, Action<int> body, CancellationToken cancellationToken)
		{
			var parallelOptions = cancellationToken.CanBeCanceled
				? new ParallelOptions { CancellationToken = cancellationToken, MaxDegreeOfParallelism = Environment.ProcessorCount }
				: DefaultParallelOptions;

			return Parallel.For(fromInclusive, toExclusive, parallelOptions, Wrapper);

			void Wrapper(int i)
			{
				try
				{
					body(i);
				}
				catch (OperationCanceledException e) when (cancellationToken.IsCancellationRequested && e.CancellationToken != cancellationToken)
				{
					cancellationToken.ThrowIfCancellationRequested();
					throw new UnreachableException();
				}
			}
		}
	}
}