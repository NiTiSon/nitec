using System;

namespace NiteCompiler.CodeAnalysis.Text;

[Obsolete("Use SourceLocation instead.")]
public record SourceSpan
{
	public SourceText Source { get; }
	public TextSpan Span { get; }

	public SourceSpan(SourceText source, TextSpan span)
	{
		Source = source;
		Span = span;
	}

	public string GetText() => Source.GetText(Span);

	public int Start => Span.Start;
	public int Length => Span.Length;
	public int End => Span.End;

	public static implicit operator TextSpan(SourceSpan span) => span.Span;
}