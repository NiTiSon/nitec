using System;
using Nlr.Compiler.CodeAnalysis.Syntax;
using Nlr.Compiler.CodeAnalysis.Text;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public class NiteCodeSyntaxTree : SyntaxTree
{
	public override CompilationUnit CompilationUnit { get; }

	public NiteCodeSyntaxTree(Source source)
	{
		NiteCodeLexer lexer = new(source);
		NiteCodeParser parser = new(lexer);

		CompilationUnit = null;
	}
	
}