using System;
using System.Collections.Immutable;
using System.Reflection.Emit;
using Nlr.Compiler.CodeAnalysis.Syntax;
using Nlr.Compiler.NiteCode.CodeAnalysis;
using Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;
using Nlr.Compiler.NiteCode.Symbols;
using Nlr.Compiler.Symbols;

namespace Nlr.Compiler.NiteCode;

public sealed class NiteCodeCompilation : Compilation
{
	private readonly ImmutableArray<NiteCodeSyntaxTree> _trees;
	private readonly ImmutableDictionary<NiteCodeSyntaxTree, NiteCodeSemanticModel> _semanticModels;

	public NiteCodeCompilation(string name, params ReadOnlySpan<NiteCodeSyntaxTree> trees) : base(name)
	{
		_trees = [..trees];
		_semanticModels = ImmutableDictionary<NiteCodeSyntaxTree, NiteCodeSemanticModel>.Empty;
	}
	
	public override ILibrarySymbol Library => throw new NotImplementedException();

	public override NiteCodeSemanticModel GetSemanticModel(SyntaxTree tree)
	{
		if (tree is not NiteCodeSyntaxTree syntaxTree) throw new ArgumentException(null, nameof(tree));
		
		return _semanticModels.TryGetValue(syntaxTree, out NiteCodeSemanticModel? model) 
			? model 
			: throw new ArgumentException("Tree not found in compilation.");
	}
}