using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Semantics;

public sealed class SemanticModel
{
	private readonly SyntaxTree _tree;
	private readonly SymbolTable _symbolTable;

	public SemanticModel(SyntaxTree tree)
	{
		_tree = tree;
		_symbolTable = new();
	}
}