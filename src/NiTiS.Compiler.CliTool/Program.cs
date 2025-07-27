using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Threading.Tasks;
using NiTiS.Compiler.CodeAnalysis.Syntax;
using NiTiS.Compiler.CodeAnalysis.Text;
using NiTiS.Compiler.Diagnostics;
using NiTiS.Compiler.NiteCode;
using NiTiS.Compiler.NiteCode.CodeAnalysis.Syntax;

namespace NiTiS.Compiler.CliTool;

public static class Program
{
	public static void Main(string[] args)
	{
		Console.WriteLine("[" + string.Join(", ", args) + "]");

		Argument<FileInfo[]> inputArgument = new("files")
		{
			Description = "Source files",
		};

		RootCommand rootCommand = new("Nite compiler CLI tool.");
		rootCommand.Arguments.Add(inputArgument);
		rootCommand.SetAction(result => Compile(result.GetValue(inputArgument)));

		ParseResult parseResult = rootCommand.Parse(args);
		parseResult.Invoke();

		foreach (ParseError parseError in parseResult.Errors)
		{
			Console.Error.WriteLine(parseError.Message);
		}
	}

	private static void Compile(FileInfo[]? files)
	{
		if (files is null)
		{
			Environment.Exit(-1);
		}

		if (!CheckForNoDuplicates(files))
		{
			Environment.Exit(-2);
		}

		Parallel.ForEachAsync(files, static async (file, token) =>
		{
			Stopwatch stopwatch =  new();
			await using FileStream stream = file.OpenRead();
			DiagnosticBag diagnostics = new();

			SHA256 sha =  SHA256.Create();
			byte[] hashValue = await sha.ComputeHashAsync(stream, token);
			Console.WriteLine(Convert.ToHexString(hashValue));
			NiteLexer fileLexer = new(
				diagnostics,
				new StringText(await File.ReadAllTextAsync(file.FullName, token))
			);

			stopwatch.Start();
			NiteToken lexeme = fileLexer.Lex();
			while (lexeme.Kind != SyntaxKind.EndOfFile)
			{
				Console.WriteLine(lexeme);
				lexeme = fileLexer.Lex();
			}
			stopwatch.Stop();

			foreach (Diagnostic diagnostic in diagnostics)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Error.WriteLine(diagnostic);
				Console.ResetColor();
			}
			Console.WriteLine("Lexer: " + stopwatch.Elapsed);
		}).Wait();
	}

	private static bool CheckForNoDuplicates(FileInfo[] files)
	{
		HashSet<FileInfo> verified = new(files.Length, FileInfoFullNameComparer.Instance);

		return files.All(file => verified.Add(file));
	}
}