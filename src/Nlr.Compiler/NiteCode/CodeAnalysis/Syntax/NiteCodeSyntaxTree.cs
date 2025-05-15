using System;
using System.Diagnostics;
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
		DebugPrintNode(compilationUnit, 0);
	}
	
	private static void DebugPrintNode(ISyntaxNode node, int depth)
	{
		string padding = new(' ', depth * 4);

		if (node is Token token)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine($"{padding}{node.Kind} '{token.Value}'");
			Console.ResetColor();
		}
		else
		{
			Console.ForegroundColor = ConsoleColor.DarkBlue;
			Console.WriteLine($"{padding}{node.Kind}");
			Console.ResetColor();
		}
    
		foreach (ISyntaxNode child in node.GetChildren())
		{
			DebugPrintNode(child, depth + 1);
		}
	}
}