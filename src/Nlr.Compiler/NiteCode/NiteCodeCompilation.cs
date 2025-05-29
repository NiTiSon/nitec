using System;
using System.Collections.Immutable;
using Nlr.Compiler.CodeAnalysis.Syntax;
using Nlr.Compiler.NiteCode.CodeAnalysis;
using Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;
using Nlr.Compiler.NiteCode.Symbols;

namespace Nlr.Compiler.NiteCode;

public sealed class NiteCodeCompilation : Compilation
{
	private ImmutableArray<NiteCodeSyntaxTree> _trees;

	public NiteCodeCompilation(params ReadOnlySpan<NiteCodeSyntaxTree> trees)
	{
		_trees = [..trees];
	}
	
	public override ILibrarySymbol Library => throw new NotImplementedException();

	public override NiteCodeSemanticModel GetSemanticModel(SyntaxTree tree)
	{
		NiteCodeSemanticModel semanticModel = null!;


		return semanticModel;
	}
}