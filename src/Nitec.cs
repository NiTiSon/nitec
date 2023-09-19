using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using GlobExpressions;

namespace NiteCompiler;

/// <summary>
/// NiteCode Compiler.
/// </summary>
internal static class Nitec
{
	public static void Main(string[] args)
	{
		var opt = Parameters.Parse(args);

		List<string> inputFiles = new();

		if (opt.InputFiles.Length == 0)
		{
			inputFiles.AddRange(Glob.Files(Environment.CurrentDirectory, "*.nite"));
		}
		else
		{
			foreach (string file in opt.InputFiles)
			{
				if (new char[]{ '?', '*', '[' }.Any(c => file.Contains(c)))
				{
					inputFiles.AddRange(Glob.Files(Environment.CurrentDirectory, file));
				}
				else
				{
					if (File.Exists(file))
						inputFiles.Add(file);
				}
			}
		}

#if DEBUG
		Console.WriteLine(opt.OutputPath);
		Console.WriteLine(opt.LanguageVersion ?? "<default>");
		Console.WriteLine($"Defines: [{string.Join(", ", opt.Defines)}]");
		Console.WriteLine($"Input: {{{string.Join(", ", opt.InputFiles)}}}");
		Console.WriteLine($"Found Input: {{{string.Join(", ", inputFiles)}}}");
#endif

		if (inputFiles.Count > 0)
		{
		}
		else
		{
			using FileStream fs = File.Create(opt.OutputPath);
			using BinaryWriter bw = new(fs);

			// Header: Signature
			bw.Write((byte)0x6Eu); // The magic number is Big-Endian
			bw.Write((byte)0x6Cu);
			bw.Write((byte)0x69u);
			bw.Write((byte)0x62u);
			bw.Write((ushort)1u);  // File format version
			bw.Write((ushort)0u);  // Reserved

			// Header: Execution Info
			bw.Write(0u); // Entry point id
			bw.Write(0u); // First segment offset

			// Header: Bytecode information
			bw.Write(0u); // Instruction pack version
			bw.Write(0u); // Reserved

			// Header: Reserved
			bw.Write(0ul);
		}
	}

	[DoesNotReturn]
	public static void CriticalExitWithError(string error)
	{
		ConsoleColor clr = Console.ForegroundColor;
		Console.ForegroundColor = ConsoleColor.Red;

		Console.WriteLine(error);

		Console.ForegroundColor = clr;
		Environment.Exit(201);
	}
}
