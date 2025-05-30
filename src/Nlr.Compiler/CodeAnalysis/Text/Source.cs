using System;

namespace Nlr.Compiler.CodeAnalysis.Text;

public abstract class Source : IDisposable
{
	/// <summary>
	/// Copy a range of characters from this SourceText to a destination array.
	/// </summary>
	public abstract void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count);

	public virtual void Dispose() { }

	public abstract int Length { get; }
}