using System.Collections.Generic;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public class EmptyStatementSyntax : StatementSyntax
{
	public override SyntaxKind Kind => SyntaxKind.EmptyStatement;
	public override IEnumerable<ISyntaxNode> GetChildren()
	{
		yield break;
	}
}