using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.Diagnostics;

internal sealed class SourceLocation(SyntaxTree tree, TextSpan span) : Location
{
	public override SyntaxTree SyntaxTree => tree;
	public override TextSpan? Span => span;
}