﻿namespace NiteCode.Compiler.CodeAnalysis.Text;

public readonly struct TextLocation(SourceText source, TextSpan span)
{
	public SourceText Text { get; } = source;
	public TextSpan Span { get; } = span;

	public override string ToString()
	{
		return Text.Text.Substring((int)Span.Position, (int)Span.Length);
	}
}