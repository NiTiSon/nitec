using System;
using System.Threading;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.Diagnostics;

internal sealed class SourceLocation : Location
{
	private readonly Lazy<SyntaxTree> _syntaxTree;

	public override SyntaxTree SourceTree => _syntaxTree.Value;
	public override TextSpan Span { get; }

	public SourceLocation(SyntaxTree source, TextSpan span)
	{
		_syntaxTree = new(source);
		Span = span;
	}

	public SourceLocation(Func<SyntaxTree> source, TextSpan span)
	{
		_syntaxTree = new(source, LazyThreadSafetyMode.None);
		Span = span;
	}
}