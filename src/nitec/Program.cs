﻿using NiteCode.Compiler.CodeAnalysis.Syntax;
using System;
using System.IO;

internal static class Program
{
	public static void Main(string[] args)
	{
		string filePath;
		if (args.Length == 0)
		{
			filePath = Console.ReadLine()!;
		}
		else
		{
			filePath = args[0];
		}

		if (!File.Exists(filePath))
		{
			Console.Error.WriteLine("Filename doesn't exists.");
			Environment.Exit(1);
		}

		SyntaxTree tree = SyntaxTree.Load(filePath);
	}
}