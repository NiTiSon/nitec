using System;
using System.Diagnostics;
using System.Linq;
using Nlr.Compiler.CodeAnalysis.Syntax;
using Nlr.Compiler.CodeAnalysis.Text;
using Nlr.Compiler.Diagnostics;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public class NiteCodeSyntaxTree : SyntaxTree
{
	public sealed override CompilationUnit CompilationUnit { get; }

	public NiteCodeSyntaxTree(Source source)
	{
		DiagnosticBag diagnostics = new();

		Stopwatch stopwatch = Stopwatch.StartNew();
		NiteCodeLexer lexer = new(diagnostics, source);
		Console.WriteLine($"Tree:Lexer[{source}]: {stopwatch.Elapsed}");
		stopwatch.Restart();
		NiteCodeParser parser = new(diagnostics, lexer);
		Console.WriteLine($"Tree:Parser[{source}]: {stopwatch.Elapsed}");

		CompilationUnit = parser.ParseCompilationUnit();
		DebugPrint(CompilationUnit);

		Console.WriteLine("=== DIAGNOSTICS ===");
		foreach (Diagnostic diagnostic in diagnostics)
		{
			Console.WriteLine(diagnostic.ToString());
		}
		Console.WriteLine("=== END OF DIAGNOSTICS ===");
	}

	private static void DebugPrint(CompilationUnit compilationUnit)
	{
		// Print the root node separately
		Console.ForegroundColor = ConsoleColor.DarkBlue;
		Console.WriteLine($"{compilationUnit.Kind}");
		Console.ResetColor();

		var children = compilationUnit.GetChildren().ToList();
		for (int i = 0; i < children.Count; i++)
		{
			bool isLast = i == children.Count - 1;
			DebugPrintNode(children[i], "", isLast);
		}
	}

	private static void DebugPrintNode(ISyntaxNode node, string indent, bool isLast)
	{
		// Draw tree connectors
		string connector = isLast ? "└── " : "├── ";
		Console.Write(indent);
		Console.Write(connector);

		// Print node content
		if (node is Token token)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine($"{node.Kind} '{token.Value}'");
			Console.ResetColor();
		}
		else
		{
			Console.ForegroundColor = ConsoleColor.DarkBlue;
			Console.WriteLine($"{node.Kind}");
			Console.ResetColor();
		}

		// Build indentation for children
		string childIndent = indent + (isLast ? "    " : "│   ");

		// Recursively print children
		var children = node.GetChildren().ToList();
		for (int i = 0; i < children.Count; i++)
		{
			bool childIsLast = i == children.Count - 1;
			DebugPrintNode(children[i], childIndent, childIsLast);
		}
	}
}