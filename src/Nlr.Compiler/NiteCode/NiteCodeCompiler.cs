using System;
using System.Threading;
using System.Threading.Tasks;
using Nlr.Compiler.CodeAnalysis.Syntax;
using Nlr.Compiler.CodeAnalysis.Text;
using Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode;

public class NiteCodeCompiler : CommonCompiler
{
	public NiteCodeCompiler(BuildPaths buildPaths) : base(buildPaths)
	{
	}

	public override Compilation? CreateCompilation(string[] sourceFiles)
	{
		SyntaxTree[] syntaxTrees = new SyntaxTree[sourceFiles.Length];
		
		Parallel.For(0L, sourceFiles.Length, i =>
		{
			syntaxTrees[i] = ParseFile(sourceFiles[i]);
		});

		return null;
	}

	private static SyntaxTree ParseFile(string sourceFile)
	{
		return new NiteCodeSyntaxTree(new FileSource(sourceFile));
	}
}