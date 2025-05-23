using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public struct EqualsValueClause
{
	public Token EqualsToken { get; } 
	public ExpressionSyntax Value { get; }

	public EqualsValueClause(Token equalsToken, ExpressionSyntax value)
	{
		EqualsToken = equalsToken;
		Value = value;
	}
}