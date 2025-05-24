using System.Collections.Generic;
using System.Runtime.InteropServices;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class LocalVariableDeclarationSyntax : StatementSyntax
{
	public Token LetKeyword { get; }
	public SimpleNameSyntax Identifier { get; }
	public NameSyntax? TypeName { get; init; }
	public EqualsValueClause? ValueClause { get; init; }

	public LocalVariableDeclarationSyntax(Token letKeyword, SimpleNameSyntax name, NameSyntax? typeName, EqualsValueClause? valueClause)
	{
		LetKeyword = letKeyword;
		Identifier = name;
		TypeName = typeName;
		ValueClause = valueClause;
	}

	public override SyntaxKind Kind => SyntaxKind.LocalVariableDeclaration;
	public override IEnumerable<ISyntaxNode> GetChildren()
	{
		yield return LetKeyword;
		yield return Identifier;
		if (TypeName != null)
		{
			yield return TypeName;
		}

		if (ValueClause != null)
		{
			yield return ValueClause!.Value.EqualsToken;
			yield return ValueClause!.Value.Value;
		}
	}
}