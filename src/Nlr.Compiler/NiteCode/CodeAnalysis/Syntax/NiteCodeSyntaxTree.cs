using System;
using Nlr.Compiler.CodeAnalysis.Syntax;
using Nlr.Compiler.CodeAnalysis.Text;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public class NiteCodeSyntaxTree : SyntaxTree
{
	public override CompilationUnit CompilationUnit { get; }

	public NiteCodeSyntaxTree(Source source)
	{
		NiteCodeLexer lexer = new(source);

		Token token;
		do
		{
			token = lexer.Lex();
			
			Console.WriteLine($"{token}");
		}
		while (token.Kind != SyntaxKind.EndOfFile);

		CompilationUnit = null;
	}
}