namespace NiteCompiler.CodeAnalysis.Text;

public record SourceSpan
{
	public SourceText Source { get; }
	public TextSpan Span { get; }

	public SourceSpan(SourceText source, TextSpan span)
	{
		Source = source;
		Span = span;
	}
}