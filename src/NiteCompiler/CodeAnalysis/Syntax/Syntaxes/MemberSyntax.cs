namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class MemberSyntax : ItemSyntax
{
	private protected MemberSyntax(SyntaxTree tree) : base(tree)
	{
	}
}