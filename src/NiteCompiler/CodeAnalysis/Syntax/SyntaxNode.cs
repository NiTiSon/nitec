using System;
using System.Collections.Generic;
using System.Reflection;
using NiteCompiler.CodeAnalysis.Text;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class SyntaxNode
{
	public SyntaxTree SyntaxTree { get; internal set; } = null!;
	public abstract TextSpan Span { get; }
	public abstract SyntaxKind Kind { get; }
	public abstract IEnumerable<SyntaxNode> GetChildren();

	[Obsolete("Use Location instead.")]
	public SourceSpan ContextualizedSpan => Span.Contextualize(SyntaxTree.Text);
	public Location Location => Location.Create(SyntaxTree, Span);

	public IEnumerable<Token> GetTokens(bool includeThisToken = false)
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
				foreach (Token child in node.GetTokens(true))
				{
					yield return child;
				}
			}
		}
	}

	public IEnumerable<SyntaxNode> GetNodes(bool includeThisToken = false)
	{
		if (includeThisToken)
			yield return this;

		foreach (SyntaxNode node in GetChildren())
		{
			if (node is Token)
			{
				yield return node;
			}
			else
			{
				foreach (SyntaxNode child in node.GetNodes(true))
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