using System.Collections.Generic;
using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class FunctionDeclarationSyntax : MemberSyntax
{
	public NameSyntax Name { get; }
	public RetusaSyntax? Retusa { get; }
	public BlockStatementSyntax Block { get; }

	public FunctionDeclarationSyntax(Token accessibilityToken, ImmutableArray<Token> modifiers, NameSyntax name, object todoParamList,
		RetusaSyntax? retusa, BlockStatementSyntax block) : base(accessibilityToken, modifiers)
	{
		Name = name;
		Retusa = retusa;
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