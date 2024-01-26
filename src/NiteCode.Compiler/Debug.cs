using NiteCode.Compiler.CodeAnalysis.Syntax;
using System.Runtime.CompilerServices;

namespace NiteCode.Compiler;

internal static class Debug
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Ensure(SyntaxToken token, SyntaxKind kind)
	{
#if DEBUG
		if (token.Kind != kind)
			throw new System.Exception($"Token {token} is not {kind}");
#endif
	}
}