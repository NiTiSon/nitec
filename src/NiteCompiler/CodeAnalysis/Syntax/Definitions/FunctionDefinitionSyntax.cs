using NiteCompiler.CodeAnalysis.Syntax.Expressions.Names;
using NiteCompiler.CodeAnalysis.Syntax.Statements;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax.Definitions;

public class FunctionDefinitionSyntax : DefinitionSyntax
{
	public NameSyntax Name { get; }
	public Token AccessibilityToken { get; }
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
}