using System.Collections.Generic;
using System.Runtime.InteropServices;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class QualifiedTypeNameSyntax : NameSyntax
{
	public NameSyntax Left { get; }
	public Token DotToken { get; }
	public SimpleNameSyntax Right { get; }

	public QualifiedTypeNameSyntax(NameSyntax left, Token dotToken, SimpleNameSyntax right)
	{
		Left = left;
		DotToken = dotToken;
		Right = right;
	}

	public override SyntaxKind Kind => SyntaxKind.QualifiedName;

	public override IEnumerable<ISyntaxNode> GetChildren()
	{
		yield return Left;
		yield return DotToken;
		yield return Right;
	}
}