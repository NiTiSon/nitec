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

	// Diagnostics / debug emission
	public static readonly Option<bool> EmitNir;
	public static readonly Option<bool> EmitAst;
	public static readonly Option<bool> EmitLlvmIr;
	public static readonly Option<bool> EmitLlvmBc;

	static Options()
	{
		Input = new Argument<FileInfo[]>("sources")
		{
			Arity = ArgumentArity.ZeroOrMore,
			Description = "Input source files written in Nite."
		};

		Dependencies = new Option<FileInfo[]>("-d", "--dependencies")
		{
			Description = "Include libraries for compilation."
		};

		AllWarningsAsErrors = new Option<bool>("-wae", "--all-warning-as-error")
		{
			Description = "Treat all warnings as errors."
		};

		WarningsAsErrors = new Option<string[]>("-wae+", "--warning-as-error")
		{
			Arity = ArgumentArity.OneOrMore,
			Description = "Treat following warnings as errors."
		};

		WarningsNotAsErrors = new Option<string[]>("-wae-", "--warning-as-error-exclude")
		{
			Arity = ArgumentArity.OneOrMore,
			Description = "Ignore following warnings from \"treat all warnings as errors\"."
		};

		LibraryName = new Option<string>("-n", "--name")
		{
			Description = "Name of output library."
		};

		OutputPath = new Option<string>("-o", "--output")
		{
			Description = "Path to the output file."
		};

		OverwriteOutput = new Option<bool>("-Y", "--overwrite-output")
		{
			Description = "Overwrite output file if exists."
		};

		OutputKind = new Option<string>("-k", "--output-kind")
		{
			Description =
				"Determines the output format. If not set, the tool tries to infer the output kind from the output file name;" +
				"if no output file name is provided, it falls back to the executable format."
		};
		OutputKind.AcceptOnlyFromAmong(OutputKinds);

		EmitNir = new Option<bool>("--emit-nir")
		{
			Description = "Write the Nite Intermediate Representation (SSA form) for every compiled " +
			              "function to '<outputname>.nir' alongside the primary output."
		};

		EmitAst = new Option<bool>("--emit-ast")
		{
			Description = "Write the Abstract Syntax Tree for every source file to " +
			              "'<sourcefile>.ast' alongside the primary output."
		};

		EmitLlvmIr = new Option<bool>("--emit-llvm-ir")
		{
			Description = "Write the LLVM IR (textual intermediate representation) for every compiled " +
			              "function to '<outputname>.ll'."
		};

		EmitLlvmBc = new Option<bool>("--emit-llvm-bc")
		{
			Description = "Write the LLVM bitcode for every compiled " +
			              "function to '<outputname>.bc'."
		};
	}

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

		command.Options.Add(EmitNir);
		command.Options.Add(EmitAst);
		command.Options.Add(EmitLlvmIr);
		command.Options.Add(EmitLlvmBc);
	}
}