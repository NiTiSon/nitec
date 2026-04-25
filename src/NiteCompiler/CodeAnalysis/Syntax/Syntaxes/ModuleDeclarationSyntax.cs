using System.Collections.Generic;
using System.Linq;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class ModuleDeclarationSyntax : ItemSyntax
{
	internal readonly Token LastToken;

	public Token ModuleKeyword { get; }
	public NameSyntax Name { get; }
	public Token SemicolonToken { get; }
	public SyntaxList<MemberSyntax> Members { get; }

	public override TextSpan Span
	{
		get
		{
			if (!Members.IsEmpty)
			{
				return TextSpan.FromBounds(ModuleKeyword.Span, Members.Span);
			}

			return TextSpan.FromBounds(ModuleKeyword.Span, SemicolonToken.Span);
		}
	}
	public override NodeKind Kind => NodeKind.ModuleDeclaration;

	internal ModuleDeclarationSyntax(SyntaxTree tree, Token moduleKeyword, NameSyntax name, Token semicolon, SyntaxList<MemberSyntax> members) : base(tree)
	{
		ModuleKeyword = moduleKeyword;
		Name = name;
		SemicolonToken = semicolon;
		Members = members;

		if (members.IsEmpty)
		{
			LastToken = SemicolonToken;
		}
		else
		{
			LastToken = members[^1].GetTokens().Last();
		}
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitModuleDeclaration(this);
	public override void Accept(SyntaxVisitor visitor) => visitor.VisitModuleDeclaration(this);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return ModuleKeyword;
		yield return Name;
		yield return SemicolonToken;
		yield return Members;
	}
}