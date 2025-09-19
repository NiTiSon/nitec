using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class ComplexNameSyntax : NameSyntax
{
	public override TextSpan Span { get; }
	public override SyntaxKind Kind =>  SyntaxKind.ComplexName;
	public override IEnumerable<SyntaxNode> GetChildren()
	{
		throw new System.NotImplementedException();
	}
}