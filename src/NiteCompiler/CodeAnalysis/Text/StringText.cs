using System;
using System.Runtime.CompilerServices;

namespace NiteCompiler.CodeAnalysis.Text;

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
		return _text.Substring(span.Start, span.Length);
	}

	public override int GetText(int index, Span<char> destination)
	{
		ReadOnlySpan<char> source = _text.AsSpan(index);

		if (source.Length > destination.Length)
		{
			source[..destination.Length].CopyTo(destination);
			return destination.Length;
		}

		source.CopyTo(destination);
		return source.Length;
	}

	public override char this[int index]
	{
		get
		{
			#if DEBUG
			if (index < 0 || index >= _text.Length)
			{
				throw new IndexOutOfRangeException();
			}
			#endif

			return _text[index];
		}
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
}