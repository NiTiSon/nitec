using System;
using Nlr.Compiler.CodeAnalysis.Text;

namespace Nlr.Compiler;

public abstract class CommonCompiler
{
	private readonly BuildPaths _buildPaths;

	private protected CommonCompiler(BuildPaths buildPaths)
	{
		_buildPaths = buildPaths;
	}

	public virtual Compilation? CreateCompilationFromFilePaths(params ReadOnlySpan<string> filePaths)
	{
		FileSource[] fileSources = new FileSource[filePaths.Length];

		for (int i = filePaths.Length - 1; i >= 0; i--)
		{
			fileSources[i] = new FileSource(filePaths[i]);
		}
		
		return CreateCompilation(fileSources);
	}

	public abstract Compilation? CreateCompilation(params ReadOnlySpan<Source> sourceFiles);
}