using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiteCompiler;

internal class Parameters
{
	private string outputPath;
	private string[] inputFiles;
	private Dictionary<string, string?> defines;
	private string languageVersion;

	public string OutputPath => outputPath;
	public string[] InputFiles => inputFiles;
	public IReadOnlyDictionary<string, string?> Defines => defines;
	public string LanguageVersion => languageVersion;

	public Parameters()
	{
		outputPath = "a.nlib";
		defines = new Dictionary<string, string?>();
	}
	public static Parameters Parse(string[] args)
	{
		Parameters parameters = new();
		List<string> inputList = new();
		uint threadsCount = 0;
		string? outputName = null;

		for (int i = 0; i < args.Length; i++)
		{
			string arg = args[i];
			if (arg.Length == 0)
				continue;
			
			if (arg[0] == '-')
			{
				switch (arg)
				{
					case "-o":
					case "--output":
						if (args.IsNextAvilable(i))
						{
							outputName = args[i + 1];
						}
						else
						{
							Nitec.CriticalExitWithError($"Argument '{arg}' (output path) has no parameters of type 'string'");
						}

						break;
					case "-t":
					case "--threads":
						if (args.IsNextAvilable(i))
						{
							if (!(UInt32.TryParse(args[i + 1], out threadsCount) && threadsCount <= 256))
								Nitec.CriticalExitWithError($"Parameter '{args[i + 1]}' is out of bounds or invalid\nAllowed values: [0...256]");
						}
						else
						{
							Nitec.CriticalExitWithError($"Argument '{arg}' (threads count) has no parameters of type 'u8'");
						}

						break;
					default:
						if (arg.StartsWith("-d") || arg.StartsWith("--define"))
						{
							Nitec.CriticalExitWithError("Define parameter is unsupported yet.");
							continue;
						}

						inputList.Add(arg);
						break;
				}
			}
			else
			{
				inputList.Add(arg);
			}
		}

		parameters.inputFiles = inputList.ToArray();

		if (outputName is not null)
			parameters.outputPath = outputName;

		return parameters;
	}

}
internal static class ArrayExtensions
{
	public static bool IsNextAvilable<T>(this T[] array, int index)
		=> index < array.Length - 1;
}