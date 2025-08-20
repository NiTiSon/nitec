using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Syntax.Expressions.Names;
using NiteCompiler.CodeAnalysis.Syntax.Statements;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax.Definitions;

public class FunctionDefinitionSyntax : DefinitionSyntax
{
	public Token AccessibilityToken { get; }
	public NameSyntax Name { get; }
	public BlockStatementSyntax Block { get; }

	public FunctionDefinitionSyntax(Token accessibilityToken, NameSyntax name, object todoParamList,
		BlockStatementSyntax block)
	{
		Name = name;
		AccessibilityToken = accessibilityToken;
		Block = block;
	}
	public override TextSpan Span => TextSpan.FromBounds(AccessibilityToken.Span.Start, Block.Span.End);

	public override SyntaxKind Kind => SyntaxKind.FunctionDeclaration;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return AccessibilityToken;
		yield return Name;
		yield return Block;
	}
}