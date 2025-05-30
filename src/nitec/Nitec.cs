using Nlr.Compiler;
using System;
using System.CommandLine;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Nlr.Compiler.CodeAnalysis.Text;
using Nlr.Compiler.NiteCode;

public sealed class Nitec : NiteCodeCompiler
{
	private Nitec(BuildPaths buildPaths) : base(buildPaths) {}

	public static int Main(string[] args)
	{
#if DEBUG
		Console.OutputEncoding = System.Text.Encoding.UTF8;
		Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
#endif
		string version = typeof(Compilation).Assembly.GetCustomAttributes<AssemblyInformationalVersionAttribute>().First().InformationalVersion;
		RootCommand rootCommand = new($"Compilation tool for NLR languages.");

		Argument<FileInfo[]?> inputArgument = new("input", "Input source code files")
		{
			Arity = ArgumentArity.OneOrMore,
		};

		Option<FileInfo?> outOption = new(["--out", "-o"], "Path to resulting file")
		{
			ArgumentHelpName = "output",
			Arity = ArgumentArity.ExactlyOne,
		};

		Option<FileInfo[]?> includeOption = new(["-l", "--libs"], "Path to included runtime-time dependencies")
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

		if (args.Length == 0)
		{
			Console.WriteLine($"""
			NiTiSon (R) NiteCode & Nibc Compiler version {version}
			Copyright (C) NiTiSon Hentaiev. All rights reserved.
			""");
		}

		return rootCommand.Invoke(args);
		
	}

	public static void Process(FileInfo[]? inputFiles, FileInfo[]? includedFiles, FileInfo outFile)
	{
		Nitec nitec = new(new BuildPaths(Environment.CurrentDirectory));

		NiteCodeCompilation? compilation = nitec.CreateCompilation("__notimpl", inputFiles?.Select(t => new FileSource(t)).ToArray() ?? []);
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