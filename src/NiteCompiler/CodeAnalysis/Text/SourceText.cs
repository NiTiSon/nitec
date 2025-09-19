using System;

namespace NiteCompiler.CodeAnalysis.Text;

public abstract class SourceText : IDisposable
{
	public string? FileName { get; }

	protected SourceText(string? fileName)
	{
		FileName = fileName;
	}

	/// <summary>
	/// Copy a range of characters from this SourceText to a destination array.
	/// </summary>
	public abstract void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count);

	public virtual void Dispose() { }

	public abstract string GetText(TextSpan span);

	public abstract string GetText(TextLine line);

	public abstract int Length { get; }

	public TextSpan Span =>  new(0, Length);

	public virtual char this[int i] => GetText(new TextSpan(i, 1))[0];
}