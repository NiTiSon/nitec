using System.CommandLine;
using System.IO;

namespace NiteCompiler.CliTool;

internal static class Options
{
	// Input
	public static readonly Argument<FileInfo[]> Input;
	public static readonly Option<FileInfo[]> Dependencies;

	// Language-related
	public static readonly Option<bool> AllWarningsAsErrors;
	public static readonly Option<string[]> WarningsAsErrors;
	public static readonly Option<string[]> WarningsNotAsErrors;

	// Output
	public static readonly Option<string> LibraryName;
	public static readonly Option<string> OutputPath;
	public static readonly Option<bool> OverwriteOutput;
	public static readonly Option<string> OutputKind;
	public const string ExecutableOutputKindName = "executable";
	public const string NiTiSLibraryOutputKindName = "nlib";
	public static readonly string[] OutputKinds = [ExecutableOutputKindName, NiTiSLibraryOutputKindName];

	// public static Option<bool> EmitLlvmIrCode;
	// public static Option<bool> EmitLlvmBitCode;

	public static void ConfigureCompileCommand(Command command)
	{
		command.Arguments.Add(Input);
		command.Options.Add(Dependencies);

		command.Options.Add(AllWarningsAsErrors);
		command.Options.Add(WarningsAsErrors);
		command.Options.Add(WarningsNotAsErrors);

		command.Options.Add(LibraryName);
		command.Options.Add(OutputPath);
		command.Options.Add(OverwriteOutput);
		command.Options.Add(OutputKind);
	}

	static Options()
	{
		Input = new("sources")
		{
			Arity = ArgumentArity.ZeroOrMore,
			Description = "Input source files written in Nite."
		};

		Dependencies = new("-d", "--dependencies")
		{
			Description = "Include libraries for compilation."
		};

		AllWarningsAsErrors = new("-wae", "--all-warning-as-error")
		{
			Description = "Treat all warnings as errors."
		};

		WarningsAsErrors = new("-wae+", "--warning-as-error")
		{
			Arity = ArgumentArity.OneOrMore,
			Description = "Treat following warnings as errors."
		};

		WarningsNotAsErrors = new("-wae-", "--warning-as-error-exclude")
		{
			Arity = ArgumentArity.OneOrMore,
			Description = "Ignore following warnings from \"treat all warnings as errors\"."
		};

		LibraryName = new("-n", "--name")
		{
			Description = "Name of output library.",
		};

		OutputPath = new("-o", "--output")
		{
			Description = "Path to the output file."
		};

		OverwriteOutput = new("-Y", "--overwrite-output")
		{
			Description = "Overwrite output file if exists."
		};

		OutputKind = new("-k", "--output-kind")
		{
			Description = "Determines the output format. If not set, the tool tries to infer the output kind from the output file name;" +
			              "if no output file name is provided, it falls back to the executable format."
		};
		OutputKind.AcceptOnlyFromAmong(OutputKinds);
	}
}