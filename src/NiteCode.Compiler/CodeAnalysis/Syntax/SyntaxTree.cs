using NiteCode.Compiler.CodeAnalysis.Text;
using System;
using System.IO;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public sealed class SyntaxTree
{
	private SyntaxTree(SourceText source)
	{
		Text = source;
	}

	public SourceText Text { get; }
	//public CompilationUnitSyntax Root { get; }

	public static SyntaxTree Load(string filePath)
		=> Load(new FileInfo(filePath));

	public static SyntaxTree Load(FileInfo file)
	{
		if (!file.Exists)
			throw new FileNotFoundException(null, file.FullName);

		StreamReader sr = file.OpenText();
		string content = sr.ReadToEnd();
		sr.Close();

		return Load(file.FullName, content);
	}

	private static SyntaxTree Load(string? filePath, string content)
	{
		SourceText source = new(content, filePath);
		SyntaxTree tree = new(source);

		DateTime startMeasure = DateTime.UtcNow;
		Parser parser = new(tree);

		//CompilationUnitSyntax unit = parser.ParseCompilationUnit();
		ExpressionSyntax bin = parser.ParseExpression()!;
		DateTime endMeasure = DateTime.UtcNow;

		TimeSpan diff = endMeasure - startMeasure;

		Console.WriteLine($"Lexer+Parser time: {diff:ss\\:ffffff}");

		foreach (Diagnostic diagnostic in parser.Diagnostics)
		{
			Console.WriteLine($"{diagnostic.Id}: {diagnostic.Message}");
		}

		bin.PrettyPrint(Console.Out);

		return tree;
	}
}
