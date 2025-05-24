using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis;

public abstract class SemanticModel
{
	public Compilation Compilation { get; }
	public SyntaxTree Tree { get; }

	protected SemanticModel(Compilation compilation, SyntaxTree tree)
	{
		Compilation = compilation;
		Tree = tree;
	}
}