using Nlr.Compiler.CodeAnalysis.NiteCode;
using Nlr.Compiler.Diagnostics;
using System;
using System.CommandLine;
using System.Diagnostics;
using System.IO;
using System.Linq;

internal static class Program
{
	public const string DefaultCacheDirectory = "./~nitec_cache/";

	public static int Main(string[] args)
	{
#if DEBUG
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
		NiteCodeSyntaxTree[] trees = new NiteCodeSyntaxTree[inputFiles?.Length ?? 0];

		if (trees.Length == 0)
		{
			// Diagnostic: no input
			return;
		}

		for (int i = 0; i < inputFiles.Length; i++)
		{
			FileInfo file = inputFiles[i];
			
			if (!file.Exists)
			{
				// Diagnostic: file doesn't exists
				continue;
			}
			
			using FileStream fs = file.OpenRead();
			using TextReader reader = new StreamReader(fs);

			trees[i] = NiteCodeSyntaxTree.ParseText(reader.ReadToEnd(), file.FullName, new NiteCodeOptions(LanguageVersion.Latest));
		}
	}

	private static void PrintDiagnostic(Diagnostic diagnostic)
	{

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