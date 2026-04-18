using System;
using System.Collections;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
			OutputKind.Executable, null, null,
			null, []);
	}

	private static void Compile(FileInfo[] sources, FileInfo[] dependencies, NiteCompilationOptions options,
		OutputKind outputKind, string? outputPath, string? libraryName,
		string? target, string[] targetFeatures)
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

		var compilation = NiteCompilation.Create(libraryName, trees!, null, options);
		compilation.Diagnostics.DrainInto(diagnostics);

		DiagnosticBag? resultingDiagnostics = null;
		try
		{
			switch (outputKind)
			{
				case OutputKind.NiTiSLibrary:
				case OutputKind.Executable:
					FileStream fs = new("./out.nlib", FileMode.Create, FileAccess.Write);
					compilation.EmitNiteLibrary(fs, out resultingDiagnostics);

					break;
				default:
					throw new NotSupportedException();
			}
		}
		catch (Exception exception)
		{
			Debug.WriteLine("Compiler unwanted exception :(");
			diagnostics.ReportInternalCompilerError(exception);
		}
		finally
		{
			resultingDiagnostics?.DrainInto(diagnostics);
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

		// Header
		Console.ForegroundColor = foreColor;
		Console.Write($"{type}[{diagnostic.Id}]");
		Console.ResetColor();
		Console.WriteLine(": " + diagnostic.Message);

		if (diagnostic.Locations.Length == 0) return;

		var locationsGroupedBySource = diagnostic.Locations
			.Sort((l, r) => l.Span?.CompareTo(r.Span) ?? -1)
			.GroupBy(d => d.SyntaxTree)
			.Select(g => (SyntaxTree: g.Key, Locations: g.ToArray()));

		foreach (var group in locationsGroupedBySource)
		{
			if (group.SyntaxTree is null) continue; // If SyntaxTree is null, then locations is definitely not within text files

			var source = group.SyntaxTree.Text;
			var lines = source.Lines;

			TextLine theVeryEnd = lines.GetLineByCharacterPosition(group.Locations[^1].Span!.Value.End)!.Value;
			string lastLineNumber = theVeryEnd.HumanReadableLineNumber.ToString();
			int gutterWidth = lastLineNumber.Length + 1;

			foreach (var location in group.Locations)
			{
				TextSpan span = location.Span!.Value;
				TextLine begin = lines.GetLineByCharacterPosition(location.Span!.Value.End)!.Value;
				TextLine end = lines.GetLineByCharacterPosition(location.Span!.Value.End)!.Value;

				Console.WriteLine($"{new string(' ', gutterWidth - 1)}--> filename:{begin.HumanReadableLineNumber}:{begin.GetColumnIndex(span.Start) + 1}");
				Console.WriteLine(new string(' ', gutterWidth) + "|");

				for (int i = begin.Index; i <= end.Index && i < source.Lines.Count; i++)
				{
					TextLine current = source.Lines.GetLineByIndex(i);
					string lineNum = current.HumanReadableLineNumber.ToString().PadLeft(gutterWidth - 1);

					// avoid IndexOutOfRange if line is empty (EOF after newline)
					string text = string.Empty;
					if (current.LineSpan.End <= source.Length && current.LineSpan.Start < source.Length)
						text = source.GetText(current.LineSpan).TrimEnd('\n', '\r');

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
			}
		}
	}
}