using System.Diagnostics.CodeAnalysis;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.Diagnostics;

public abstract class Location
{
	private protected Location() {}

	[MemberNotNullWhen(true, nameof(SourceTree))]
	public bool IsInSource => SourceTree != null;

	[MemberNotNullWhen(true, nameof(MetadataLibrary))]
	public bool IsInMetadata => MetadataLibrary != null;

	public virtual SyntaxTree? SourceTree => null;
	public virtual LibrarySymbol? MetadataLibrary => null;

	public virtual TextSpan Span => default;

	public static Location Create(SyntaxTree tree, TextSpan span)
	{
		return new SourceLocation(tree, span);
	}

	// TODO: Add Metadata and ExternalFile location
}