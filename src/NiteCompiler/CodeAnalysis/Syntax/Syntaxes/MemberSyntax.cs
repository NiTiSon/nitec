namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class MemberSyntax : ItemSyntax
{
	public SyntaxList<AttributeListSyntax>? Attributes { get; }

	private protected MemberSyntax(SyntaxTree tree, SyntaxList<AttributeListSyntax>? attributes = null) : base(tree)
	{
		Attributes = attributes;
	}
}