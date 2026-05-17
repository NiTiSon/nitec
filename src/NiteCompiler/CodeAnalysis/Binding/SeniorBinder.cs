using System.Diagnostics;
using System.Reflection;
using NiteCompiler.CodeAnalysis.Symbols;
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

	public override TypeSymbol? ContainingType => null;
}