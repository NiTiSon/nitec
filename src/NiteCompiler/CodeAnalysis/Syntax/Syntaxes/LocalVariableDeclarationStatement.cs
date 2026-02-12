using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class LocalVariableDeclarationStatement : StatementSyntax
{
	public Token DeclarationToken { get; }
	public LocalVariableDeclarator Declarator { get; }
	public Token SemicolonToken { get; }

	public override NodeKind Kind => NodeKind.LocalVariableDeclarationStatement;
	public override TextSpan Span => TextSpan.FromBounds(DeclarationToken.Span, Declarator.Span);

	public LocalVariableDeclarationStatement(SyntaxTree tree, Token declarationKeyword, LocalVariableDeclarator declarator, Token semicolonToken) : base(tree)
	{
		DeclarationToken = declarationKeyword;
		Declarator = declarator;
		SemicolonToken = semicolonToken;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return DeclarationToken;
		yield return Declarator;
		yield return SemicolonToken;
	}
}