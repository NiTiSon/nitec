using Nlr.Compiler.Text;
using System;

namespace Nlr.Compiler.CodeAnalysis.NiteCode;

public sealed class NiteCodeSyntaxTree
{
	public static NiteCodeSyntaxTree ParseText(string text, string path, NiteCodeOptions options)
	{
		NiteCodeLexer lexer = new(new SourceText(text));

		return new NiteCodeSyntaxTree();
	}
}