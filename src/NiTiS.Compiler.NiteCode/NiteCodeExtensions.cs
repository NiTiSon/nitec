using System.Collections.Immutable;
using NiTiS.Compiler.CodeAnalysis.Syntax;
using NiTiS.Compiler.CodeAnalysis.Text;
using NiTiS.Compiler.NiteCode.CodeAnalysis.Syntax;

namespace NiTiS.Compiler.NiteCode;

public static class NiteCodeExtensions
{
	extension(Token token)
	{
		public SyntaxKind Kind => (SyntaxKind)token.RawKind;
		public SyntaxKind ContextualKind => (SyntaxKind)token.RawContextualKind;
	}

	extension(Trivia trivia)
	{
		public SyntaxKind Kind => (SyntaxKind)trivia.RawKind;

		public static Trivia Create(SyntaxKind kind, TextSpan span)
		{
			return new((uint)kind, span);
		}
	}
}