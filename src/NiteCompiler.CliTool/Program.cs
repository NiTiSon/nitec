using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.CodeAnalysis.Text;
using NiTiS.Compiler.CliTool;
using NiTiS.Compiler.Diagnostics;

namespace NiteCompiler.CliTool;

public static class Program
{
	private static readonly DiagnosticBag _diagnosticBag = new();
	public static void Main(string[] args)
	{
		#if DEBUG
		Console.WriteLine("[" + string.Join(", ", args) + "]");
		#endif

		Argument<FileInfo[]> inputArgument = new("files")
		{
			Description = "Source files",
		};

		RootCommand rootCommand = new("Nite CLI compiler tool (.NET impl).");
		rootCommand.Arguments.Add(inputArgument);
		rootCommand.SetAction(result => Compile(result.GetValue(inputArgument), "test123"));

		ParseResult parseResult = rootCommand.Parse(args);
		parseResult.Configuration.EnableDefaultExceptionHandler = false;
		parseResult.Invoke();

		foreach (ParseError parseError in parseResult.Errors)
		{
			Console.Error.WriteLine(parseError.Message);
		}
	}

	private static void Compile(FileInfo[]? files, string packageName)
	{
		if (files is null || files.Length == 0)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Error.WriteLine("No input files.");
			Console.ResetColor();
			Environment.Exit(1);
		}

		if (!RemoveDuplicates(ref files))
		{
			// _diagnosticBag.Add();
		}

		SyntaxTree[] trees = new SyntaxTree[files.Length];
		Parallel.For(0, files.Length, i =>
		{
			FileInfo file = files[i];
			trees[i] = SyntaxTree.Load(file);
		});

		Compilation compilation = Compilation.Create(trees);
		foreach (SyntaxTree tree in compilation.SyntaxTrees)
		{
			Console.WriteLine(tree.Text.FileName);
			foreach (SyntaxNode node in tree.Root.TopLevelNodes)
			{
				Console.WriteLine(node);
			}
		}
	}

	private static bool RemoveDuplicates(ref FileInfo[] files)
	{
		HashSet<FileInfo> verified = new(files.Length, FileInfoFullNameComparer.Instance);

		bool hasDuplicates = !files.All(file => verified.Add(file));
		if (hasDuplicates) files = verified.ToArray();
		return hasDuplicates;
	}
}