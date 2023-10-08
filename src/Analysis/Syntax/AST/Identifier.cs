using NiteCompiler.Analysis.Syntax.Tokens;

namespace NiteCompiler.Analysis.Syntax.AST;

public class Identifier : SyntaxTree
{
	public IdentifierOrKeywordToken Name;
	public Identifier? Next;
	public Identifier(SyntaxKind kind, IdentifierOrKeywordToken name, Identifier? next) : base(kind)
	{
		Name = name;
		Next = next;
	}
}
