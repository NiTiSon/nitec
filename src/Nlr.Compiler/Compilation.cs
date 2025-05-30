using CommunityToolkit.Diagnostics;
using Nlr.Compiler.CodeAnalysis.Syntax;
using Nlr.Compiler.NiteCode.CodeAnalysis;
using Nlr.Compiler.Symbols;

namespace Nlr.Compiler;

public abstract class Compilation
{
	public string Name { get; }
	
	public abstract ILibrarySymbol Library { get; }

	protected Compilation(string name)
	{
		Guard.IsNotNullOrEmpty(name);
		Name = name;
	}
	
	public abstract SemanticModel GetSemanticModel(SyntaxTree tree);
}