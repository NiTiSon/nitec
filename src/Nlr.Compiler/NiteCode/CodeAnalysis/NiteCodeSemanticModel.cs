using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis;

public sealed class NiteCodeSemanticModel : SemanticModel
{
	public NiteCodeSemanticModel(NiteCodeCompilation compilation, SyntaxTree tree) : base(compilation, tree)
	{
	}
}