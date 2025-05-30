using System;
using Nlr.Compiler.CodeAnalysis.Text;
using Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode;

public class NiteCodeCompiler : CommonCompiler
{
	public NiteCodeCompiler(BuildPaths buildPaths) : base(buildPaths)
	{
	}

	public override NiteCodeCompilation? CreateCompilationFromFilePaths(string compilationName, params ReadOnlySpan<string> filePaths)
	{
		return base.CreateCompilationFromFilePaths(compilationName, filePaths) as NiteCodeCompilation;
	}

	public override NiteCodeCompilation? CreateCompilation(string compilationName, params ReadOnlySpan<Source> sourceFiles)
	{
		NiteCodeSyntaxTree[] syntaxTrees = new NiteCodeSyntaxTree[sourceFiles.Length];

		for (int i = sourceFiles.Length - 1; i >= 0; i--)
		{
			syntaxTrees[i] = new NiteCodeSyntaxTree(sourceFiles[i]);
		}
		
		return new NiteCodeCompilation(compilationName, syntaxTrees);
	}
}