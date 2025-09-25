using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class SyntaxNode
{
	public abstract TextSpan Span { get; }
	public abstract SyntaxKind Kind { get; }
	public abstract IEnumerable<SyntaxNode> GetChildren();

	public IEnumerable<SyntaxNode> GetTokens(bool includeThisToken = false)
	{
		if (includeThisToken && this is Token token)
			yield return token;

		foreach (SyntaxNode node in GetChildren())
		{
			if (node is Token token2)
			{
				yield return token2;
			}
			else
			{
				foreach (SyntaxNode child in node.GetTokens(true))
				{
					yield return child;
				}
			}
		}
	}

	public override string ToString()
	{
		return $"{Kind} @{Span}";
	}
}