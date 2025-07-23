using System;

namespace NiTiS.Compiler.CodeAnalysis.Text;

public sealed class StringText : SourceText
{
	private readonly string _text;

	public StringText(string text)
	{
		ArgumentNullException.ThrowIfNull(text);

		_text = text;
	}

	public override int Length => _text.Length;

	public override void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
	{
		_text.CopyTo(sourceIndex, destination, destinationIndex, count);
	}

	public override string GetText(TextSpan span)
	{
		TextSpan wholeSpan = new(0, Length);

		if (wholeSpan.OverlapsWith(span))
		{
			return _text.Substring(span.Start, span.Length);
		}

		throw new ArgumentException(null, nameof(span));
	}
}