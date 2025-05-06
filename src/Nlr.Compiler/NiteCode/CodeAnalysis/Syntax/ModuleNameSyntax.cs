using System.Collections.Immutable;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class ModuleNameSyntax : NameSyntax
{
	public ImmutableArray<Token> Identifiers { get; }

	public ModuleNameSyntax(ImmutableArray<Token> identifiers)
	{
		Identifiers = identifiers;
	}

	public override SyntaxKind Kind => SyntaxKind.ModuleName;
}