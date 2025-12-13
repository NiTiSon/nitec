using System;
using System.Diagnostics.CodeAnalysis;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.Diagnostics;

public abstract class Location
{
	private protected Location() {}

	[NotNullIfNotNull(nameof(Span))]
	public virtual SyntaxTree? SyntaxTree => null;
	public virtual TextSpan? Span => null;
	public virtual string? Filename => SyntaxTree?.Filename;

	public static Location Create(SyntaxTree tree, TextSpan span)
	{
		SourceLocation location = new(tree, span);
		return location;
	}
}