using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using GlobExpressions;
using NiteCompiler.Analysis;

namespace NiteCompiler;

/// <summary>
/// NiteCode Compiler.
/// </summary>
internal static class Nitec
{
	private static readonly object lockTasks = new();

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
		Stopwatch sw = Stopwatch.StartNew();
		#endif

		if (inputFiles.Count > 0)
		{
			AnalysisPipeline[] tasks = new AnalysisPipeline[inputFiles.Count];
			Stack<AnalysisPipeline> tasksStack = new();

			for (int i = 0; i < inputFiles.Count; i++)
			{
				AnalysisPipeline task = new(inputFiles[i]);
				tasksStack.Push(task);
				tasks[i] = task;
			}

			List<Thread> threads = new(4);
			for (int t = 0; t < Math.Min(4, inputFiles.Count); t++)
			{
				Thread thread = new(ThreadAction);
				thread.Name = $"PipelineThread{t:000}";

				threads.Add(thread);
				thread.Start();
			}

			while (threads.Any(t => t.IsAlive))
			{ }

#if DEBUG
			for (int i = 0; i < tasks.Length; i++)
			{
				AnalysisPipeline pipeline = tasks[i];
				Console.WriteLine($"Pipeline [{i:000}]: in {pipeline.MeasureTicks,7} ticks ({pipeline.MeasureSeconds:0.0000} sec.) Tokens: {pipeline.tokens?.Count}");
			}
#endif

			void ThreadAction()
			{
				AnalysisPipeline? pipeline;
				int pipelinesCount = 0;

			NEXT:
#if DEBUG
				Stopwatch sw = new();
				sw.Start();
#endif
				lock (lockTasks)
				{
					if (tasksStack.Count > 0)
					{
						pipelinesCount++;
						pipeline = tasksStack.Pop();
					}
					else
					{
#if DEBUG
						sw.Stop();
						Console.WriteLine($"Thread '{Thread.CurrentThread.Name}' #{Environment.CurrentManagedThreadId} exit with {pipelinesCount} pipelines ended.");
#endif
						return;
					}
				}

				pipeline.ReadFile();
				pipeline.Tokenize();

#if DEBUG
				sw.Stop();
				pipeline.MeasureTicks = sw.ElapsedTicks;
#endif
				goto NEXT;
			}
		}
		else
		{
			//using FileStream fs = File.Create(opt.OutputPath);
			//using BinaryWriter bw = new(fs);

			//// Header: Signature
			//bw.Write((byte)0x6Eu); // The magic number is Big-Endian
			//bw.Write((byte)0x6Cu);
			//bw.Write((byte)0x69u);
			//bw.Write((byte)0x62u);
			//bw.Write((ushort)1u);  // File format version
			//bw.Write((ushort)0u);  // Reserved

			//// Header: Execution Info
			//bw.Write(0u); // Entry point id
			//bw.Write(0u); // First segment offset

			//// Header: Bytecode information
			//bw.Write(0u); // Instruction pack version
			//bw.Write(0u); // Reserved

			//// Header: Reserved
			//bw.Write(0ul);
		}

#if DEBUG
		sw.Stop();
		Console.WriteLine($"Compilation time: {sw.ElapsedTicks} ticks ({sw.ElapsedTicks / 10_000_000d} sec.)");
#endif
	}

	[DoesNotReturn]
	public static void CriticalExitWithError(string error)
	{
		ConsoleColor clr = Console.ForegroundColor;
		Console.ForegroundColor = ConsoleColor.Red;

		Console.Error.WriteLine(error);

		Console.ForegroundColor = clr;
		Environment.Exit(201);
	}
}
