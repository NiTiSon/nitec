using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.CodeAnalysis.Text;
using NiteCompiler.Diagnostics;
using NiteLang.Metadata;

namespace NiteCompiler.CliTool;

public static class Program
{
	public static void Main(string[] args)
	{
		// using FileStream fs = new(@"C:\Users\UwU\Desktop\s.bin", FileMode.Open, FileAccess.Write);
		// fs.SetLength(0);
		// // using NlibWriter nlib = new(fs, true);
		// //
		// // NlibLibraryBuilder stdlib = new("stdlib");
		// // NlibModuleBuilder numericsModule = stdlib.CreateModule("std::numerics");
		// // NlibModuleBuilder stdModule = stdlib.CreateModule("std");
		// // NlibTypeBuilder sint8 = numericsModule.CreateType("SInt8");
		// // NlibTypeBuilder sint16 = numericsModule.CreateType("SInt16");
		// // NlibTypeBuilder sint32 = numericsModule.CreateType("SInt32");
		// // NlibTypeBuilder sint64 = numericsModule.CreateType("SInt64");
		// // NlibTypeBuilder uint8 = numericsModule.CreateType("UInt8");
		// // NlibTypeBuilder uint16 = numericsModule.CreateType("UInt16");
		// // NlibTypeBuilder uint32 = numericsModule.CreateType("UInt32");
		// // NlibTypeBuilder uint64 = numericsModule.CreateType("UInt64");
		// //
		// // nlib.Write(stdlib);
		// return;
		#if DEBUG
		Console.WriteLine("[DEBUG] Input arguments: [" + string.Join(", ", args) + "]");
		Stopwatch stopwatch = Stopwatch.StartNew();
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

		#if DEBUG
		stopwatch.Stop();
		Console.WriteLine("[DEBUG] Code analysis time: {0:g}", stopwatch.Elapsed);
		#endif
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
			Thread.CurrentThread.Name = $"Compile thread[{i}]: {file.Name}";
			trees[i] = SyntaxTree.Load(file);
			diagnostics.AddRange(trees[i].Diagnostics);
		});

		Compilation compilation = new(libraryName, [], trees);
		compilation.Diagnostics.DrainInto(diagnostics);

		foreach (SyntaxTree tree in compilation.SyntaxTrees)
		{
			PrintTree(tree);
		}

		if (diagnostics.IsEmpty) return;

		foreach (Diagnostic diagnostic in diagnostics)
		{
			WriteDiagnostic(diagnostic);
		}
	}

	private static void WriteDiagnostic(Diagnostic diagnostic)
	{
		if (diagnostic.Severity == DiagnosticSeverity.Hidden)
			return;

		(string type, ConsoleColor foreColor) = diagnostic.Severity switch
		{
			DiagnosticSeverity.Error => ("error", ConsoleColor.Red),
			DiagnosticSeverity.Warning => ("warning", ConsoleColor.Yellow),
			DiagnosticSeverity.Info => ("info", ConsoleColor.Cyan),
			_ => throw new ArgumentException(null, nameof(diagnostic))
		};

		// header
		Console.ForegroundColor = foreColor;
		Console.Write($"{type}[{diagnostic.Id}]");
		Console.ResetColor();
		Console.WriteLine(": " + diagnostic.Message);

		if (diagnostic.Span == null)
			return;

		var span = diagnostic.Span;
		var source = span.Source;
		var lines = source.Lines;

		TextLine? beginOpt = lines.GetLineByCharacterPosition(span.Start);
		TextLine? endOpt = lines.GetLineByCharacterPosition(span.End);

	if (beginOpt == null || endOpt == null)
	{
		// Defensive fallback for weird spans (e.g. after EOF)
		Console.ForegroundColor = ConsoleColor.DarkGray;
		Console.WriteLine($"(invalid span {span.Start}..{span.End})");
		Console.ResetColor();
		return;
	}

		TextLine begin = beginOpt.Value;
		TextLine end = endOpt.Value;

		string firstLineNum = begin.HumanReadableLineNumber.ToString();
		int gutterWidth = firstLineNum.Length + 1;

		Console.WriteLine($"{new string(' ', gutterWidth - 1)}--> {source.FileName}:{begin.HumanReadableLineNumber}:{begin.GetColumnIndex(span.Start) + 1}");
		Console.WriteLine(new string(' ', gutterWidth) + "|");

		for (int i = begin.Index; i <= end.Index && i < source.Lines.Count; i++)
		{
			TextLine current = source.Lines.GetLineByIndex(i);
			string lineNum = current.HumanReadableLineNumber.ToString().PadLeft(gutterWidth - 1);

			// avoid IndexOutOfRange if line is empty (EOF after newline)
			string text = string.Empty;
			if (current.LineSpan.End <= source.Length && current.LineSpan.Start < source.Length)
				text = source.GetText(current.LineSpan).TrimLineTerminators();

			Console.Write(lineNum);
			Console.Write(" | ");

			if (text.Length == 0)
			{
				Console.WriteLine();
				continue;
			}

			int startOffset = 0;
			int endOffset = text.Length;

			if (i == begin.Index)
				startOffset = Math.Clamp(span.Start - current.Position, 0, text.Length);
			if (i == end.Index)
				endOffset = Math.Clamp(span.End - current.Position, startOffset, text.Length);

			Console.Write(text[..startOffset]);
			Console.ForegroundColor = foreColor;
			Console.Write(text[startOffset..endOffset]);
			Console.ResetColor();
			Console.WriteLine(text[endOffset..]);
		}

		Console.WriteLine(new string(' ', gutterWidth) + "|");
}


	private static void PrintTree(SyntaxTree tree, string indent = "", bool isLast = true)
	{
		if (tree.Root.TopLevelNodes.Length == 0) return;

		Console.WriteLine(tree.Text.FileName ?? "<unnamed>");

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