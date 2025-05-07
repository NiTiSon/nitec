using System;
using Nlr.Compiler.CodeAnalysis.Syntax;
using Nlr.Compiler.CodeAnalysis.Text;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public class NiteCodeSyntaxTree : SyntaxTree
{
	public override CompilationUnit CompilationUnit { get; }

	public NiteCodeSyntaxTree(Source source)
	{
		NiteCodeLexer lexer = new(source);
		NiteCodeParser parser = new(lexer);

		CompilationUnit = parser.ParseCompilationUnit();
		DebugPrint(CompilationUnit);
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
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine($"{padding}{node.Kind} '{token.Value}'");
			Console.ResetColor();
		}
		else
		{
			Console.WriteLine($"{padding}{node.Kind}");
		}
    
		foreach (ISyntaxNode child in node.GetChildren())
		{
			DebugPrintNode(child, depth + 1);
		}
	}
}