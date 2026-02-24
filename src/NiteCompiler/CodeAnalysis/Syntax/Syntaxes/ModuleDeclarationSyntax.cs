using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class ModuleDeclarationSyntax : SyntaxNode
{
	public Token ModuleKeyword { get; }
	public NameSyntax Name { get; }
	public SyntaxList<MemberSyntax> Members { get; }
	public override TextSpan Span => Members.Span;
	public override NodeKind Kind => NodeKind.ModuleDeclaration;

	public ModuleDeclarationSyntax(SyntaxTree tree, Token moduleKeyword, NameSyntax name, SyntaxList<MemberSyntax> members) : base(tree)
	{
		ModuleKeyword = moduleKeyword;
		Members = members;
		Name = name;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitModuleDeclaration(this);
	public override void Accept(SyntaxVisitor visitor) => visitor.VisitModuleDeclaration(this);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return ModuleKeyword;
		yield return Name;
		yield return Members;
	}
}