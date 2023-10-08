using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using NiteCompiler.Analysis;
using NiteCompiler.Analysis.Syntax.Tokens;
using NiteCompiler.Analysis.Text;

namespace NiteCompiler;

/// <summary>
/// NiteCode Compiler.
/// </summary>
internal static class Nitec
{
	public static void Main(string[] args)
	{
		var opt = Parameters.Parse(args);

		foreach (string file in opt.InputFiles)
		{
			string content = File.ReadAllText(file);

			DiagnosticBag bag = new();
			Lexer lexer = new(LanguageOptions.StandardV1, new CodeSource(content, file), bag);

			List<Token> tokens = new();
			while (!lexer.IsEOF)
			{
				tokens.Add(lexer.NextToken());
			}

			if (bag.Any())
			{
				ConsoleColor def = Console.ForegroundColor;
				foreach (Diagnostic diagnostic in bag)
				{
					if (diagnostic.IsError)
					{
						Console.ForegroundColor = ConsoleColor.Red;
					}
					Console.WriteLine($"{diagnostic.Source} @{diagnostic.Location}: {diagnostic.Code}: {diagnostic.Message}");
					if (diagnostic.IsError)
					{
						Console.ForegroundColor = def;
					}
				}
			}

			foreach (Token token in tokens)
			{
				Console.WriteLine($"{token}");
			}
		}
	}

	[DoesNotReturn]
	public static void CriticalExitWithError(string error)
	{
		ConsoleColor clr = Console.ForegroundColor;
		Console.ForegroundColor = ConsoleColor.Red;

		Console.Error.WriteLine(error);

		Console.ForegroundColor = clr;
		Environment.Exit(201);
	}
}
