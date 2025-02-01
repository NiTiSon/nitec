using Nlr.Compiler.CodeAnalysis.NiteCode;
using Nlr.Compiler.Diagnostics;
using System;
using System.CommandLine;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Primitives;
using Nlr.Compiler.Text;

internal static class Program
{
	public const string DefaultCacheDirectory = "./~nitec_cache/";

	public static int Main(string[] args)
	{
#if DEBUG
		Console.OutputEncoding = System.Text.Encoding.UTF8;
		Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
#endif
		RootCommand rootCommand = new("Compilation tool for NLR languages.");

		Argument<FileInfo[]?> inputArgument = new("input", "Input source code files")
		{
			Arity = ArgumentArity.OneOrMore,
		};

		Option<FileInfo?> outOption = new(["--out", "-o"], "Path to resulting file")
		{
			Arity = ArgumentArity.ExactlyOne
		};

		Option<FileInfo[]?> includeOption = new(["-l"], "Path to included libraries")
		{
			ArgumentHelpName = "libraries",
			Arity = ArgumentArity.ZeroOrMore,
		};

		Option<bool?> threadWarningsAsErrorsFlag = new(["--warnaserror", "-werr"], "Treat all warnings as errors")
		{
			Arity = ArgumentArity.Zero,
		};

		Option<string[]?> disabledWarningsOption = new(["--nowarn"], "Disables selected warnings")
		{
			ArgumentHelpName = "codes",
			Arity = ArgumentArity.OneOrMore,
		};

		//Option<Architecture> architectureOption = new(["--arch"], "")
		//{
		//	ArgumentHelpName = "architecture",
		//	Arity = ArgumentArity.ExactlyOne,
		//};

		//Option<Target> targetOption = new(["--target", "-t"], () => Target.Library, "")
		//{
		//	ArgumentHelpName = "target type",
		//	Arity = ArgumentArity.ExactlyOne,
		//};

		rootCommand.Add(inputArgument);

		rootCommand.Add(outOption);
		rootCommand.Add(includeOption);
		rootCommand.Add(threadWarningsAsErrorsFlag);
		rootCommand.Add(disabledWarningsOption);
		//rootCommand.Add(architectureOption);
		//rootCommand.Add(targetOption);

		rootCommand.SetHandler(Process, inputArgument, includeOption, outOption);

		return rootCommand.Invoke(args);
	}
	
	private static void Process(FileInfo[]? inputFiles, FileInfo[]? includeFiles, FileInfo? outputFile)
	{
		DiagnosticBag globalDiagnostics = new();
		NiteCodeSyntaxTree[] trees = new NiteCodeSyntaxTree[inputFiles?.Length ?? 0];

		if (trees.Length == 0)
		{
			globalDiagnostics.Report(NiteCodeDiagnostics.NoInputFiles);
			return;
		}

		for (int i = 0; i < trees.Length; i++)
		{
			FileInfo file = inputFiles![i];
			
			if (!file.Exists)
			{
				// Diagnostic: file doesn't exists
				continue;
			}
			
			trees[i] = NiteCodeSyntaxTree.Parse(file, new NiteCodeOptions(LanguageVersion.Latest));
		}

		foreach (Diagnostic diagnostic in globalDiagnostics)
		{
			PrintDiagnostic(diagnostic);
		}

		foreach (NiteCodeSyntaxTree tree in trees)
		{
			foreach (Diagnostic diagnostic in tree.Diagnostics)
			{
				PrintDiagnostic(diagnostic);
			}
		}
	}

	private static void PrintDiagnostic(Diagnostic diagnostic)
	{
		switch (diagnostic.Severity)
		{
			case Severity.Error:
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write($"error[{diagnostic.Descriptor.Id}] ");
				Console.ResetColor();
				Console.WriteLine(diagnostic.Descriptor.Message);
				break;
		}

		if (diagnostic.Location != null)
		{
			LineList lines = new(diagnostic.Location.Content);
			LinePositionSpan span = lines.GetLinePositionSpan(diagnostic.Span);
			Console.WriteLine($"  --> {diagnostic.Location.Path}:{span.Begin}");

			if (span.IsMultiline)
			{
				
			}
			else
			{
				(Line line, uint index) = lines.GetLine(diagnostic.Span.Begin);
				String lineContent = diagnostic.Location.Substring(line.Begin, line.Length).ToString();
				lineContent = lineContent.ReplaceLineEndings(string.Empty);
				Console.WriteLine("   |");
				Console.Write($"{index + 1,-3}| {lineContent[..(int)(diagnostic.Span.Begin - line.Begin)]}");
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write(lineContent[(int)(diagnostic.Span.Begin - line.Begin)..(int)(diagnostic.Span.End - line.Begin)]);
				Console.ResetColor();
				Console.WriteLine(lineContent[(int)(diagnostic.Span.End - line.Begin)..]);
				Console.WriteLine($"   |");
			}
		}
	}

	/* === ERROR DISPLAY ===
	 * warning: unused variable
	 *   --> relative_path:line:column
	 *    |
	 * 13 | let x = 4;
	 *    | ^^^^^^^^^^
	 *    = note: #pragma warning unused_variables off
	 */
}