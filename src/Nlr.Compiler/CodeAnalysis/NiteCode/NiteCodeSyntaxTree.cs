using Nlr.Compiler.Text;
using System;
using System.Collections.Immutable;
using System.IO;
using Nlr.Compiler.Diagnostics;

namespace Nlr.Compiler.CodeAnalysis.NiteCode;

public sealed class NiteCodeSyntaxTree
{
	public DiagnosticBag Diagnostics { get; }
	
	private NiteCodeSyntaxTree(DiagnosticBag diagnostics)
	{
		Diagnostics = diagnostics;
	}

	public static NiteCodeSyntaxTree Parse(FileInfo file, NiteCodeOptions options)
	{
		DiagnosticBag diagnostics = new DiagnosticBag();
		NiteCodeLexer lexer = new(new SourceText(file), diagnostics);

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

		return new NiteCodeSyntaxTree(diagnostics);
	}
}