using NiteCode.Compiler.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public sealed class SyntaxTree
{
	private SyntaxTree(SourceText source)
	{
		Text = source;
	}

	public SourceText Text { get; }
	//public CompilationUnitSyntax Root { get; }

	public static SyntaxTree Load(string filePath)
		=> Load(new FileInfo(filePath));

	public static SyntaxTree Load(FileInfo file)
	{
		if (!file.Exists)
			throw new FileNotFoundException(null, file.FullName);

		using FileStream fs = file.OpenRead();
		using StreamReader sr = new(fs, true);

		string content = sr.ReadToEnd();

		SourceText source = new(content, file.FullName);
		SyntaxTree tree = new(source);

		Lexer lexer = new(tree);
		List<SyntaxToken> tokens = [];
		while (true)
		{
			SyntaxToken token = lexer.NextToken();

			if (token.Kind == SyntaxKind.EndOfFile)
			{
				foreach (SyntaxToken _token in tokens)
				{
					_token.WriteTo(Console.Out);
				}
				break;
			}
			else
			{
				tokens.Add(token);
			}
		}

		return tree;
	}
}