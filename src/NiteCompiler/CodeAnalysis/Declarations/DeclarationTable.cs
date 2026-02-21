using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Compilation;

namespace NiteCompiler.CodeAnalysis.Declarations;

internal sealed class DeclarationTable
{
	public ImmutableArray<SyntaxTree> Trees { get; }
	private MergedModuleDeclaration? _lateinitRoot;

	public DeclarationTable(ImmutableArray<SyntaxTree> trees)
	{
		Trees = trees;
	}

	public MergedModuleDeclaration GetMergedRoot(NiteCompilation compilation)
	{
		Debug.Assert(compilation.Declarations == this);
		if (_lateinitRoot == null)
		{
			Interlocked.CompareExchange(ref _lateinitRoot, CalculateMergedRoot(compilation), null);
		}
		return _lateinitRoot;
	}

	private MergedModuleDeclaration CalculateMergedRoot(NiteCompilation compilation)
	{
		var builder = ImmutableArray.CreateBuilder<SingleModuleDeclaration>();

		foreach (var tree in Trees)
		{
			builder.Add(DeclarationTreeBuilder.ForTree(tree));
		}

		return MergedModuleDeclaration.Create(builder.ToImmutable());
	}
}