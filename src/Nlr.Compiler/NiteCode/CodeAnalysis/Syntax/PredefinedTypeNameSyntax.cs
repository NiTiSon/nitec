using System.Collections.Generic;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public class PredefinedTypeNameSyntax : NameSyntax
{
	public Token TypeToken { get; }

	public PredefinedTypeNameSyntax(Token typeToken)
	{
		TypeToken = typeToken;
	}

	public override SyntaxKind Kind => SyntaxKind.PredefinedType;

	public override IEnumerable<ISyntaxNode> GetChildren()
	{
		yield return TypeToken;
	}
}