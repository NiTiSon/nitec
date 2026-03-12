using System.Collections.Generic;
using System.Text;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class ModuleNameSyntax : NameSyntax
{
	public SyntaxList<SimpleNameSyntax> Parts { get; }
	public override NodeKind Kind => NodeKind.ModuleNameExpression;
	public override TextSpan Span => Parts.Span;

	public ModuleNameSyntax(SyntaxTree tree, SyntaxList<SimpleNameSyntax> parts) : base(tree)
	{
		Parts = parts;
	}

	public override string GetName()
	{
		return Parts[^1].GetName();
	}

	public override string GetFullName()
	{
		StringBuilder sb = new();
		for (int i = 0; i < Parts.Count - 1; i++)
		{
			sb.Append(Parts[i].GetFullName());
			sb.Append("::");
		}
		// Last
		{
			sb.Append(Parts[^1].GetFullName());
		}
		return sb.ToString();
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitModuleName(this);
	}

	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitModuleName(this);
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Parts;
	}
}