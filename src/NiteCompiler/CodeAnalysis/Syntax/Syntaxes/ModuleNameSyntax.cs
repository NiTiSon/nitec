using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Syntax.Expressions.Names;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class ModuleNameSyntax : NameSyntax
{
	public SyntaxList<IdentifierNameSyntax> Parts { get; }

	public ModuleNameSyntax(SyntaxList<IdentifierNameSyntax> parts)
	{
		Parts = parts;
	}

	public override TextSpan Span => Parts.Span;
	public override SyntaxKind Kind => SyntaxKind.ModuleName;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		return Parts.GetChildren();
	}
}