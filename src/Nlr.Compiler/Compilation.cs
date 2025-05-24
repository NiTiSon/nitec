using System;
using Nlr.Compiler.CodeAnalysis.Syntax;
using Nlr.Compiler.NiteCode.CodeAnalysis;

namespace Nlr.Compiler;

public abstract class Compilation
{
	public abstract void AddSyntaxTrees(params ReadOnlySpan<SyntaxTree> trees);
	public abstract SemanticModel GetSemanticModel(SyntaxTree tree);
}