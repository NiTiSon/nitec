using System.Collections.Generic;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class FunctionDeclarationSyntax : MemberSyntax
{
	public Token AccessibilityToken { get; }
	public SyntaxList<Token> Modifiers { get; }
	public ParameterListSyntax ParameterList { get; }
	public SimpleNameSyntax Name { get; }
	public TypeClause? ReturnTypeClause { get; }
	public SyntaxList<LifetimeOrGenericConstraintClauseSyntax>? ConstraintClauses { get; }
	public FunctionBodySyntax Body { get; }

	public Token ClosingToken => Body.ClosingToken;

	public override TextSpan Span => TextSpan.FromBounds(AccessibilityToken.Span, Body.Span);
	public override NodeKind Kind => NodeKind.FunctionDeclaration;

	internal FunctionDeclarationSyntax(SyntaxTree tree, Token accessibilityToken, SyntaxList<Token> modifiers,
		ParameterListSyntax parameterList, SimpleNameSyntax name, TypeClause? returnTypeClause,
		SyntaxList<LifetimeOrGenericConstraintClauseSyntax>? constraintClauses, FunctionBodySyntax body) : base(tree)
	{
		AccessibilityToken = accessibilityToken;
		Modifiers = modifiers;
		Name = name;
		ReturnTypeClause = returnTypeClause;
		ConstraintClauses = constraintClauses;
		Body = body;
		ParameterList = parameterList;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitFunctionDeclaration(this);
	public override void Accept(SyntaxVisitor visitor) => visitor.VisitFunctionDeclaration(this);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return AccessibilityToken;
		yield return Modifiers;
		yield return ParameterList;
		yield return Name;
		if (ReturnTypeClause != null) yield return ReturnTypeClause;
		yield return Body;
	}
}