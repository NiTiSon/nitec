using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding.BoundTree;

internal class BoundFunctionBody : BoundNode
{
	public SyntaxNode FunctionDeclaration { get; }
	public BoundBlock BlockBody { get; }

	public BoundFunctionBody(SyntaxNode functionDeclaration, BoundBlock blockBody, bool hasErrors = false)
		: base(functionDeclaration, hasErrors || blockBody.HasErrors)
	{
		FunctionDeclaration = functionDeclaration;
		BlockBody = blockBody;
	}

	public override BoundKind Kind => BoundKind.FunctionBody;
}