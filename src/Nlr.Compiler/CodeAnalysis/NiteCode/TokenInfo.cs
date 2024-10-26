using Microsoft.Extensions.Primitives;

namespace Nlr.Compiler.CodeAnalysis.NiteCode;

internal struct TokenInfo
{
	public SyntaxKind Kind;
	public StringSegment Text;
}