using System;
using System.Collections.Generic;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public sealed class AssignmentExpressionSyntax : ExpressionSyntax
{
	public AssignmentExpressionSyntax(SyntaxTree tree, NameSyntax name, SyntaxToken @operator, ExpressionSyntax value) : base(tree)
	{
		Name = name;
		Operator = @operator;
		Value = value;

		Kind = OperatorToAssignmentKind(@operator.Kind);
	}

	public override SyntaxKind Kind { get; }
	public NameSyntax Name { get; }
	public SyntaxToken Operator { get; }
	public ExpressionSyntax Value { get; }

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Name;
		yield return Operator;
		yield return Value;
	}

	private static SyntaxKind OperatorToAssignmentKind(SyntaxKind kind)
	{
		return kind switch
		{
			SyntaxKind.EqualsToken => SyntaxKind.SimpleAssignmentExpression,
			_ => throw new Exception("Operator is invalid for assignment expression"),
		};
	}
}