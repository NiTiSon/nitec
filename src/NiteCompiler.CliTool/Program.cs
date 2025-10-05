using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.CodeAnalysis.Text;
using NiteCompiler.Diagnostics;
using NiTiS.Compiler.CliTool;

namespace NiteCompiler.CliTool;

public static class Program
{
	public static void Main(string[] args)
	{
		#if DEBUG
		Console.WriteLine("[" + string.Join(", ", args) + "]");
		#endif

		Console.OutputEncoding = Encoding.UTF8;

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

	private static void Compile(FileInfo[]? files, string libraryName)
	{
		if (files is null || files.Length == 0)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Error.WriteLine("No input files.");
			Console.ResetColor();
			Environment.Exit(1);
		}
		DiagnosticBag diagnostics = [];

		if (RemoveDuplicates(ref files))
		{
			diagnostics.ReportDuplicateSourceFiles();
		}

		SyntaxTree[] trees = new SyntaxTree[files.Length];
		Parallel.For(0, files.Length, i =>
		{
			FileInfo file = files[i];
			trees[i] = SyntaxTree.Load(file);
			diagnostics.AddRange(trees[i].Diagnostics);
		});

		Compilation compilation = new(libraryName, trees);
		compilation.Diagnostics.DrainInto(diagnostics);

		foreach (SyntaxTree tree in compilation.SyntaxTrees)
		{
			PrintTree(tree);
		}

		if (!diagnostics.IsEmpty)
		{
			Console.WriteLine("=== DIAGNOSTICS ===");
			foreach (Diagnostic diagnostic in diagnostics)
			{
				Console.WriteLine(diagnostic);
			}
		}
	}

	private static void PrintTree(SyntaxTree tree, string indent = "", bool isLast = true)
	{
		if (tree.Root.TopLevelNodes.Length == 0) return;

		Console.WriteLine(tree.Text.FileName ?? "<unknown>");

		SyntaxNode lastChild = tree.Root.TopLevelNodes[^1];

		foreach (SyntaxNode child in tree.Root.TopLevelNodes)
			PrintNode(child, indent, child == lastChild);
	}

	private static void PrintNode(SyntaxNode node, string indent = "", bool isLast = true)
	{

		string tokenMarker = isLast ? "└──" : "├──";

		Console.Write(indent);
		Console.Write(tokenMarker);
		Console.WriteLine(node);

		indent += isLast ? "   " : "│  ";

		SyntaxNode? lastChild = node.GetChildren().LastOrDefault();

		foreach (SyntaxNode child in node.GetChildren())
			PrintNode(child, indent, child == lastChild);
	}

	private static bool RemoveDuplicates(ref FileInfo[] files)
	{
		HashSet<FileInfo> verified = new(files.Length, FileInfoFullNameComparer.Instance);

		bool hasDuplicates = !files.All(file => verified.Add(file));
		if (hasDuplicates) files = verified.ToArray();
		return hasDuplicates;
	}
}