using System.Collections.Immutable;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public interface IAccessZone
{
	public ImmutableArray<UsingSyntax> Usings { get; }
}