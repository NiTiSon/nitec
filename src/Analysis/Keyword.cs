using NiteCompiler.Analysis.Syntax;

namespace NiteCompiler.Analysis;

public record struct Keyword(SyntaxKind Kind, bool Strict)
{
	public static implicit operator (SyntaxKind kind, bool strict)(Keyword value)
	{
		return (value.Kind, value.Strict);
	}

	public static implicit operator Keyword((SyntaxKind kind, bool strict) value)
	{
		return new Keyword(value.kind, value.strict);
	}
}