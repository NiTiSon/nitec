using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.Diagnostics;

internal sealed class SourceLocation : Location
{
	public override SyntaxTree SourceTree { get; }
	public override TextSpan Span { get; }

	public SourceLocation(SyntaxTree source, TextSpan span)
	{
		SourceTree = source;
		Span = span;
	}
}