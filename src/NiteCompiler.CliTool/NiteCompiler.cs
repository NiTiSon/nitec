using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLVMSharp.Interop;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.CodeAnalysis.Text;
using NiteCompiler.Compilation;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CliTool;

public static class NiteCompiler
{
	private static void Main(string[] args)
	{
		Trace.Listeners.Add(new ConsoleTraceListener());
		Console.OutputEncoding = Encoding.UTF8;

		RootCommand rootCommand = new("Nite CLI compiler tool.");
		Options.ConfigureCompileCommand(rootCommand);

		rootCommand.SetAction(Compile);

		ParseResult parseResult = rootCommand.Parse(args);
		parseResult.Configuration.EnableDefaultExceptionHandler = false;
		parseResult.Invoke();
	}

	private static void Compile(ParseResult result)
	{
		foreach (ParseError parseError in result.Errors)
		{
			Console.Error.WriteLine(parseError.Message);
		}

		FileInfo[] inputFiles = result.GetValue(Options.Input) ?? [];
		FileInfo[] dependencies = result.GetValue(Options.Dependencies) ?? [];
		string? libraryName = result.GetValue(Options.LibraryName);
		string? outputPath = result.GetValue(Options.OutputPath);

		Compile(inputFiles, dependencies, NiteCompilationOptions.Default,
			OutputKind.Executable, outputPath, libraryName,
			null, []);
	}

	private static unsafe void EmitObjectFile(LLVMModuleRef module, LLVMTargetMachineRef targetMachine,
		LLVMTargetDataRef targetData, string outputPath)
	{
		LLVM.SetModuleDataLayout(module, targetData); // no safe realization?

		if (!targetMachine.TryEmitToFile(module, outputPath, LLVMCodeGenFileType.LLVMObjectFile, out string message))
			throw new Exception($"Emit error: {message}");
	}

	private static bool LinkExecutable(string objPath, string targetTriple, string outputPath, DiagnosticBag diagnostics)
	{
		bool isMsvc = targetTriple.Contains("MSVC", StringComparison.CurrentCultureIgnoreCase);
		Process process = new()
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = isMsvc ? "clang-cl" : "clang",
				Arguments = $"{objPath} -o {outputPath} /link /subsystem:console",
				RedirectStandardError = true,
				RedirectStandardOutput = true,
				UseShellExecute = false
			}
		};

		process.Start();
		process.WaitForExit();

		if (process.ExitCode != 0)
		{
			string error = process.StandardError.ReadToEnd();
			diagnostics.ReportLinkerNotZeroReturnCode(error);
			return false;
		}

		return true;
	}

	private static void Compile(FileInfo[] sources, FileInfo[] dependencies, NiteCompilationOptions options,
		OutputKind outputKind, string? outputPath, string? libraryName,
		string? targetTriple, string[] targetFeatures)
	{
		DiagnosticBag diagnostics = [];
		outputPath ??= Environment.CurrentDirectory;

		//Debug.Assert(outputPath != null && outputKind == OutputKind.None);
		SyntaxTree?[] trees = new SyntaxTree[sources.Length];
		string?[] normalizedNames = new string?[sources.Length];
		if (options.ConcurrentBuild)
		{
			Parallel.For(0, sources.Length, i =>
			{
				trees[i] = ParseFile(options, sources[i].FullName, diagnostics, out normalizedNames[i]);
			});
		}
		else
		{
			for (int i = 0; i < sources.Length; i++)
			{
				trees[i] = ParseFile(options, sources[i].FullName, diagnostics, out normalizedNames[i]);
			}
		}

		if (ContainDuplicates(normalizedNames!))
		{
			diagnostics.ReportDuplicateSourceFiles();
		}

		if (diagnostics.HasAnyErrors)
		{
			ReportDiagnostics(diagnostics);
			return;
		}

		Debug.Assert(trees.All(t => t is not null));

		// Fallback library name
		if (libraryName == null)
		{
			string? name = normalizedNames.FirstOrDefault(t => t is not null);

			libraryName = Path.GetFileNameWithoutExtension(name) ?? "a"; // ah
		}

		var compilation = NiteCompilation.Create(libraryName, trees!, null, options, diagnostics);
		var parseDiagnostics = compilation.GetParseDiagnostics();
		var declarationDiagnostics = compilation.GetDeclarationDiagnostics();
		var compilationDiagnostics = compilation.GetFunctionBodyDiagnostics();
		diagnostics.AddRange(parseDiagnostics);
		diagnostics.AddRange(declarationDiagnostics);
		diagnostics.AddRange(compilationDiagnostics);

		if (parseDiagnostics.All(d => d.Severity != DiagnosticSeverity.Error) &&
		    declarationDiagnostics.All(d => d.Severity != DiagnosticSeverity.Error) &&
		    compilationDiagnostics.All(d => d.Severity != DiagnosticSeverity.Error))
		{
			DiagnosticBag? resultingDiagnostics = null;
			try
			{
				switch (outputKind)
				{
					case OutputKind.NiTiSLibrary:
					{
						FileStream fs = new("./out.nlib", FileMode.Create, FileAccess.Write);
						compilation.EmitNiteLibrary(fs, out resultingDiagnostics);
						break;
					}
					case OutputKind.Executable:
					{
						var (module, machine, dataLayout) = compilation.GetLlvmModule(out resultingDiagnostics, ref targetTriple);
						EmitObjectFile(module, machine, dataLayout, "out.obj");
						LinkExecutable("out.obj", targetTriple, "out.exe", diagnostics);
						break;
					}
					default:
						throw new NotSupportedException();
				}
			}
			catch (Exception exception)
			{
				Console.Error.WriteLine("=== COMPILER INTERNAL ERROR, THAT'S NOT YOUR MISTAKE ===");
				diagnostics.ReportInternalCompilerError(exception);
			}
			finally
			{
				resultingDiagnostics?.DrainInto(diagnostics);
			}
		}

		if (!diagnostics.IsEmpty)
		{
			ReportDiagnostics(diagnostics);
		}
	}

	private static SyntaxTree? ParseFile(NiteCompilationOptions options, string path, DiagnosticBag diagnostics, out string normalizedPath)
	{
		SourceText? content = TryReadFileContent(path, diagnostics, out normalizedPath);

		if (content == null)
		{
			return null;
		}

		SyntaxTree tree = SyntaxTree.ParseText(content, path, options);
		return tree;
	}

	private static SourceText? TryReadFileContent(string path, DiagnosticBag diagnostics, out string normalizedPath)
	{
		try
		{
			FileInfo info = new(path);
			normalizedPath = info.FullName;

			if (!info.Exists)
			{
				diagnostics.ReportFileDoesNotExists(normalizedPath);
				return null;
			}

			using FileStream stream = File.Open(info.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			using TextReader textReader = new StreamReader(stream);
			string text = textReader.ReadToEnd();
			return SourceText.FromText(text);
		}
		catch (Exception ex) when (ex is DirectoryNotFoundException or FileNotFoundException)
		{
			diagnostics.ReportFileDoesNotExists(path);
		}
		catch (UnauthorizedAccessException)
		{
			diagnostics.ReportHaveNoPrivilegesToReadFile(path);
		}
		catch
		{
			diagnostics.ReportUnableToOpenFile(path);
		}

		normalizedPath = path;
		return null;
	}

	private static string ReplaceForbiddenFileNameCharacters(string fileName, char replacement = ' ')
	{
		Span<char> newFileName = stackalloc char[fileName.Length];
		fileName.CopyTo(newFileName);
		foreach (char forbidden in Path.GetInvalidFileNameChars())
		{
			for (int i = 0; i < fileName.Length; i++)
			{
				if (fileName[i] == forbidden)
				{
					newFileName[i] = replacement;
				}
			}
		}

		return newFileName.ToString();
	}

	private static bool ContainDuplicates(string[] paths)
	{
		HashSet<string> verified = new(paths.Length, StringComparer.Ordinal);

		bool hasDuplicates = !paths.All(verified.Add);
		return hasDuplicates;
	}

	private static void ReportDiagnostics(DiagnosticBag diagnostics)
	{
		foreach (Diagnostic diagnostic in diagnostics)
		{
			ReportDiagnostic(diagnostic);
		}
	}

	private static void ReportDiagnostic(Diagnostic diagnostic)
	{
		(string type, ConsoleColor foreColor) = diagnostic.Severity switch
		{
			DiagnosticSeverity.Error => ("error", ConsoleColor.Red),
			DiagnosticSeverity.Warning => ("warning", ConsoleColor.Yellow),
			DiagnosticSeverity.Info => ("info", ConsoleColor.Cyan),
			_ => throw new ArgumentException(null, nameof(diagnostic))
		};

		// the amount of additional lines
		const int contextLines = 1;

		// header
		Console.ForegroundColor = foreColor;
		Console.Write($"{type}[{diagnostic.Id}]");
		Console.ResetColor();
		Console.WriteLine(": " + diagnostic.Message);

		if (diagnostic.Locations.Length == 0) return;

		var locationsGroupedBySource = diagnostic.Locations
			.OrderBy(l => l.Span!.Value.Start)
			.GroupBy(d => d.SyntaxTree)
			.Select(g => (SyntaxTree: g.Key, Locations: g.ToArray()));

		foreach (var group in locationsGroupedBySource)
		{
			if (group.SyntaxTree is null) continue; // if SyntaxTree is null, then locations is definitely not within text files

			var source = group.SyntaxTree.Text;
			var lines = source.Lines;

			TextLine theVeryEnd = lines.GetLineByCharacterPosition(group.Locations[^1].Span!.Value.End)!.Value;
			string lastLineNumber = theVeryEnd.HumanReadableLineNumber.ToString();
			int gutterWidth = lastLineNumber.Length + 1;

			foreach (var location in group.Locations)
			{
				TextSpan span = location.Span!.Value;
				TextLine startLine = lines.GetLineByCharacterPosition(location.Span!.Value.Start)!.Value;
				TextLine endLine = lines.GetLineByCharacterPosition(location.Span!.Value.End)!.Value;
				int windowStart = Math.Max(0, startLine.Index - contextLines);
				int windowEnd = Math.Min(lines.Count - 1, endLine.Index + contextLines);

				Console.WriteLine($"{new string(' ', gutterWidth - 1)}--> {location.Filename ?? "<ommited filename>"}:{startLine.HumanReadableLineNumber}:{startLine.GetColumnIndex(span.Start) + 1}");
				// Console.WriteLine(new string(' ', gutterWidth) + "|");

				for (int i = windowStart; i <= windowEnd; i++)
				{
					TextLine line = lines.GetLineByIndex(i);
					string lineNum = line.HumanReadableLineNumber.ToString().PadLeft(gutterWidth - 1);

					string text = string.Empty;
					if (line.LineSpan.End <= source.Length && line.LineSpan.Start < source.Length)
					{
						text = source.GetText(line.LineSpan).TrimEnd('\r', '\n');
					}

					Console.Write(lineNum);
					Console.Write(" | ");

					if (text.Length == 0)
					{
						Console.WriteLine();
						continue;
					}

					bool isErrorLine = i >= startLine.Index && i <= endLine.Index;

					if (!isErrorLine)
					{
						Console.WriteLine(text.Replace("\t", "    "));
						continue;
					}

					int highlightStart = 0;
					int highlightEnd = text.Length;

					if (i == startLine.Index)
					{
						highlightStart = Math.Clamp(span.Start - line.Position, 0, text.Length);
					}

					if (i == endLine.Index)
					{
						highlightEnd = Math.Clamp(span.End - line.Position, highlightStart, text.Length);
					}

					Console.Write(text[..highlightStart].Replace("\t", "    "));

					Console.ForegroundColor = foreColor;
					Console.Write(text[highlightStart..highlightEnd].Replace("\t", "    "));
					Console.ResetColor();

					Console.WriteLine(text[highlightEnd..].Replace("\t", "    "));
				}
			}
		}
	}
}