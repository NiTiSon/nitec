using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Compilation;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class SeniorBinder : Binder
{
	public SeniorBinder(NiteCompilation compilation, SyntaxTree syntaxTree) : base(compilation)
	{
	}

	public override Binder? GetBinder(SyntaxNode node)
	{
		return null;
	}
}