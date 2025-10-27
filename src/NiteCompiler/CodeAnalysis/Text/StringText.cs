using System;
using System.Runtime.CompilerServices;

namespace NiteCompiler.CodeAnalysis.Text;

public sealed class StringText : SourceText
{
	private readonly string _text;

	public StringText(string text, string? fileName = null) : base(fileName)
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

	public override string GetText(TextLine line)
	{
		return line.IsEmpty ? string.Empty : GetText(new TextSpan(line.Position, line.LengthIncludingLineBreak));
	}

	private readonly WeakReference<SourceLines> _lazyLines = new(null!);
	public override SourceLines Lines
	{
		get
		{
			if (_lazyLines.TryGetTarget(out SourceLines? lines))
			{
				return lines;
			}

			lines = new SourceLines(this);

			_lazyLines.SetTarget(lines);

			return lines;
		}
	}

	public override char this[int i] => _text[i];
}