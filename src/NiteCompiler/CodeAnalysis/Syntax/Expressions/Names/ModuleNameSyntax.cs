using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax.Expressions.Names;

public sealed class ModuleNameSyntax : NameSyntax
{
	public override TextSpan Span { get; }
	public override SyntaxKind Kind { get; }
	public override IEnumerable<SyntaxNode> GetChildren()
	{
		throw new System.NotImplementedException();
	}
}