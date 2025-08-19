using System;

namespace NiteCompiler.CodeAnalysis.Text;

public abstract class SourceText : IDisposable
{
	/// <summary>
	/// Copy a range of characters from this SourceText to a destination array.
	/// </summary>
	public abstract void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count);

	public virtual void Dispose() { }

	public abstract string GetText(TextSpan span);

	public abstract int Length { get; }
}