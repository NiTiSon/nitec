using System;
using System.IO;
using CommunityToolkit.Diagnostics;

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

	public static SourceText From(string text)
	{
		return new StringText(text);
	}

	public SourceText FromFile(string fileName)
	{
		return FromFile(new FileInfo(fileName));
	}

	public SourceText FromFile(FileInfo file)
	{
		if (!file.Exists)
		{
			throw new FileNotFoundException();
		}

		string text =File.ReadAllText(file.FullName);

		return new StringText(text, file.FullName);
	}
}