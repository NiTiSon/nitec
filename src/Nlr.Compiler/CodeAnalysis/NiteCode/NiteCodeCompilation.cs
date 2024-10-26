using System;
using System.Collections.Generic;
using System.IO;

namespace Nlr.Compiler.CodeAnalysis.NiteCode;

public sealed class NiteCodeCompilation
{
	public static void Compile(string path, IEnumerable<NiteCodeSyntaxTree> syntaxTrees, IEnumerable<object> referenceMetadata, NiteCodeCompilationOptions options)
	{
		FileInfo fileInfo = new(path);

		if (fileInfo.Exists)
		{
			throw new Exception($"File already exists '{path}'.");
		}

		using FileStream fs = fileInfo.Create();
	}
}