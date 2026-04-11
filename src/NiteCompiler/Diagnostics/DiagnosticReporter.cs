using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace NiteCompiler.Diagnostics;

public sealed class DiagnosticReporter
{
	public TextWriter Writer { get; }

	public DiagnosticReporter(TextWriter writer)
	{
		Writer = writer;
	}

	public void Report(DiagnosticBag diagnostics)
	{
		foreach (Diagnostic diagnostic in diagnostics)
		{
			WriteDiagnostic(diagnostic);
		}
	}

	public void Report(IEnumerable<Diagnostic> diagnostics)
	{
		foreach (Diagnostic diagnostic in diagnostics)
		{
			WriteDiagnostic(diagnostic);
		}
	}

	private void WriteDiagnostic(Diagnostic diagnostic)
	{
		if (diagnostic.Severity == DiagnosticSeverity.Hidden) return;

		(string type, ConsoleColor foreColor) = diagnostic.Severity switch
		{
			DiagnosticSeverity.Error   => ("error",   ConsoleColor.Red),
			DiagnosticSeverity.Warning => ("warning", ConsoleColor.Yellow),
			DiagnosticSeverity.Info    => ("info",    ConsoleColor.Cyan),
			_ => throw new ArgumentException(null, nameof(diagnostic))
		};

		Console.ForegroundColor = foreColor;
		Console.Write($"{type}[{diagnostic.Id}]");
		Console.ResetColor();
		Console.WriteLine(": " + diagnostic.Message);

		if (diagnostic.Locations.IsEmpty) return;

		var locationsGroupedBySource = diagnostic.Locations
			.GroupBy(d => d.SyntaxTree)
			.Select(g => new { SourceTree = g.Key, Locations = g.ToArray() });

		foreach (var group in locationsGroupedBySource)
		{
			Debug.WriteLine($"NOT IMPLEMENT REPORT, BUT HERE'S THE MESSAGE: {diagnostic.Message}");
			// TODO: re‑implement with new location API here
		}
	}
}