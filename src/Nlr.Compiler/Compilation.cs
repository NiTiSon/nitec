using Nlr.Compiler.CodeAnalysis.Syntax;
using Nlr.Compiler.NiteCode.CodeAnalysis;
using Nlr.Compiler.NiteCode.Symbols;

namespace Nlr.Compiler;

public abstract class Compilation
{
	public abstract ILibrarySymbol Library { get; }
	
	public abstract SemanticModel GetSemanticModel(SyntaxTree tree);
}