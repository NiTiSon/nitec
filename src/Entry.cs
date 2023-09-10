using System;
using System.Collections.Generic;
using System.IO;
using GlobExpressions;
using NiteCompiler.Analysis;
using NiteCompiler.Analysis.Tokens;

namespace NiteCompiler;

public static class EntryPoint
{
	public static void Main(string[] args)
	{
		string? outputName = null;
		CompilationTarget target = CompilationTarget.Library;
		string? entryPoint = null;

		List<string> files = new(args.Length);
		for (int i = 0; i < args.Length; i++)
		{
			string arg = args[i];

			if (arg.StartsWith("-"))
			{
				switch (arg)
				{
					case "-t":
					case "--target":
						if (args.Length - 1 == i)
						{
							Console.Error.WriteLine("Target parameter is empty!");
							Console.Error.WriteLine("Allowed values: [lib, obj, exe]");
							Environment.Exit(2);
						}
						else
						{
							switch (args[i + 1].ToLowerInvariant())
							{
								case "lib":
								case "library":
									target = CompilationTarget.Library;
									break;
								case "obj":
								case "object":
									target = CompilationTarget.ObjectFile;
									break;
								case "exe":
								case "executable":
									target = CompilationTarget.ExecutableImage;
									break;
								default:
									Console.Error.WriteLine("Target parameter is invalid!");
									Console.Error.WriteLine("Allowed values: [lib, obj, exe]");
									Environment.Exit(3);
									break;
							}
							i++; // Skip next argument
							continue;
						}
						break;
					case "-o":
					case "--output":
						if (args.Length - 1 == i)
						{
							Console.Error.WriteLine("Output parameter is empty!");
							Environment.Exit(1);
						}
						else
						{
							outputName = args[i + 1];
							i++; // Skip next argument
							continue;
						}
						break;
					default:
						Console.Error.WriteLine($"Unknown flag `{arg}`");
						Environment.Exit(100);
						break;
				}
			}
			else
			{
				if (arg.Contains('*') || arg.Contains('?') || arg.Contains('[') || arg.Contains(']'))
				{
					foreach (string file in Glob.Files(Environment.CurrentDirectory, arg))
					{
						files.Add(file);
					}
				}
				else
				{
					if (File.Exists(arg))
					{
						files.Add(arg);
					}
					else
					{
						Console.Error.Write($"File `{arg}` not found.");
						Environment.Exit(50);
					}
				}
			}
		}

		if (files.Count is 0)
			files.AddRange(Glob.Files(Environment.CurrentDirectory, "**/*.nite"));

		Compile(files, target, outputName ?? "unnamed%OUT_EXT%");
	}

	private static void Compile(IReadOnlyList<string> files, CompilationTarget target, string outputName)
	{
		outputName = outputName.Replace("%OUT_EXT%", target switch
		{
			CompilationTarget.Library => ".nlib",
			CompilationTarget.ExecutableImage => OperatingSystem.IsWindows() ? ".exe" : "",
			CompilationTarget.ObjectFile => ".nobj",
			_ => throw new Exception(),
		});

		Console.WriteLine($"Compile: {string.Join(',', files)} [{files.Count}]");
		Console.WriteLine($"Output: {outputName}");

		foreach (string file in files)
		{
			string content = File.ReadAllText(file);

			Lexer lexer = new(content);
			for (Token token = lexer.Next(); token.Kind is not TokenKind.EndOfFile; token = lexer.Next())
			{
				Console.WriteLine(token);
			}
		}
	}
}