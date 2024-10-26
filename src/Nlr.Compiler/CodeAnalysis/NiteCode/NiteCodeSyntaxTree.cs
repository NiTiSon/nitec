using Nlr.Compiler.Text;
using System;
using System.Collections.Immutable;

namespace Nlr.Compiler.CodeAnalysis.NiteCode;

public sealed class NiteCodeSyntaxTree
{
	public static NiteCodeSyntaxTree ParseText(string text, string path, NiteCodeOptions options)
	{
		NiteCodeLexer lexer = new(new SourceText(text));

		ImmutableArray<SyntaxToken>.Builder tokens = ImmutableArray.CreateBuilder<SyntaxToken>();
		
		SyntaxToken token;
		do
		{
			token = lexer.Lex();
			tokens.Add(token);
		}
		while (token.Kind != SyntaxKind.EndOfFile);

		for (int i = 0; i < tokens.Count; i++)
		{
			Console.WriteLine(tokens[i].ToString());
		}

		return new NiteCodeSyntaxTree();
	}
}