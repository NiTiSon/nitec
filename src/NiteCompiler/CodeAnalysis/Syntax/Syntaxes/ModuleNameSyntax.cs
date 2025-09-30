using System.Collections.Generic;
using System.Text;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class ModuleNameSyntax : SyntaxNode
{
	public SyntaxList<SimpleNameSyntax> Parts { get; }

	public ModuleNameSyntax(SyntaxList<SimpleNameSyntax> parts)
	{
		Parts = parts;
	}

	public override TextSpan Span => Parts.Span;
	public override SyntaxKind Kind => SyntaxKind.ModuleName;

	public string GetName()
	{
		// TODO: Replace with proper evaluation
		return Parts.SyntaxTree.Text.GetText(Parts.Span);
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Parts;
	}
}