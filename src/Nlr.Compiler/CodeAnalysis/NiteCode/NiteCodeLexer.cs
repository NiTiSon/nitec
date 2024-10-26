using Microsoft.Extensions.Primitives;
using Nlr.Compiler.Text;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Nlr.Compiler.CodeAnalysis.NiteCode;

internal class NiteCodeLexer : Lexer
{
	private readonly ImmutableArray<SyntaxTrivia>.Builder triviaBuilder;

	public NiteCodeLexer(SourceText source) : base(source)
	{
		triviaBuilder = ImmutableArray.CreateBuilder<SyntaxTrivia>();
	}

	public void NextToken()
	{
		LexTrivia(isTrailing: false);
		ImmutableArray<SyntaxTrivia> leadingTrivia = triviaBuilder.DrainToImmutable();

		TokenInfo tokenInfo = default;

		window.Start();
		ScanSyntaxToken(ref tokenInfo);

		LexTrivia(isTrailing: true);
		ImmutableArray<SyntaxTrivia> trailingTrivia = triviaBuilder.DrainToImmutable();
	}

	public void ScanSyntaxToken(ref TokenInfo tokenInfo)
	{
		tokenInfo.Kind = SyntaxKind.BadToken;
		tokenInfo.Text = StringSegment.Empty;
	}

	public void LexTrivia(bool isTrailing)
	{
		return;
		//while (true)
		//{
		//	window.Start();
		//	char ch = window.Peek();

		//	if (ch == ' ')
		//	{
		//		trivia.Add(ScanWhitespace());
		//		continue;
		//	}
		//}
	}
}
