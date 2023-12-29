using NiteCode.Compiler.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

		using FileStream fs = file.OpenRead();
		using StreamReader sr = new(fs, true);

		string content = sr.ReadToEnd();

		SourceText source = new(content, file.FullName);
		SyntaxTree tree = new(source);

		Parser parser = new(tree);

		CompilationUnitSyntax unit = parser.ParseCompilationUnit();

		foreach (Diagnostic diagnostic in parser.Diagnostics)
		{
			Console.WriteLine($"{diagnostic.Id}: {diagnostic.Message}");
		}

		unit.WriteTo(Console.Out);

		return tree;
	}
}