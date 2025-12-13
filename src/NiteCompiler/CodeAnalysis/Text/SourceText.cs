using System;
using System.IO;

namespace NiteCompiler.CodeAnalysis.Text;

public abstract class SourceText : IDisposable
{
	/// <summary>
	/// Copy a range of characters from this SourceText to a destination array.
	/// </summary>
	public abstract void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count);

	public virtual void Dispose() { }

	public abstract string GetText(TextSpan span);

	/// <summary>
	/// Fills <paramref name="destination"/> with text.
	/// </summary>
	/// <param name="index">Index of source text to start copy from.</param>
	/// <param name="destination">Span to fill.</param>
	/// <returns>Amount of filled characters in span.</returns>
	public abstract int GetText(int index, Span<char> destination);

	public abstract char this[int index] { get; }

	/// <summary>
	/// Returns source line collection of this source text.
	/// </summary>
	public abstract SourceLines Lines { get; }

	public abstract int Length { get; }

	public TextSpan Span =>  new(0, Length);

	public static SourceText FromText(string text)
	{
		StringText stringText = new(text);
		return stringText;
	}
}