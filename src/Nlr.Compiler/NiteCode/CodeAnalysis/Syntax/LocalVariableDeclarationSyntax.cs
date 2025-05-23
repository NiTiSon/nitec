using System.Collections.Generic;
using System.Runtime.InteropServices;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class LocalVariableDeclarationSyntax : StatementSyntax
{
	public Token LetKeword { get; }
	public SimpleNameSyntax Identifier { get; }
	public NameSyntax? TypeName { get; init; }
	public EqualsValueClause? ValueClause { get; init; }

	public LocalVariableDeclarationSyntax(Token letKeword, SimpleNameSyntax name)
	{
		LetKeword = letKeword;
		Identifier = name;
	}
	
	public override SyntaxKind Kind => SyntaxKind.LocalVariableDeclaration;
	public override IEnumerable<ISyntaxNode> GetChildren()
	{
		yield return LetKeword;
		yield return Identifier;
		if (TypeName != null)
		{
			yield return TypeName;
		}
		
	}
}