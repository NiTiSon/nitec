using System;
using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Text;
using NiTiS.Compiler.CodeAnalysis.Syntax;

namespace NiTiS.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class NiteValuableToken<T> : NiteToken
{
	public readonly string LiteralRepresentation;
	public readonly T LiteralValue;

	public NiteValuableToken(SyntaxKind kind, string representation, T value, TextSpan span, ImmutableArray<Trivia> leadingTrivia, ImmutableArray<Trivia> trailingTrivia) : base(kind, span, leadingTrivia, trailingTrivia)
	{
		LiteralRepresentation = representation;
		LiteralValue = value;
	}

	public override string ToString()
	{
		return $"Token<{typeof(T).Name}> : {LiteralRepresentation} = {LiteralValue}";
	}

	public bool IsUnsigned
		=> typeof(T) == typeof(uint)
		|| typeof(T) == typeof(ulong)
		|| typeof(T) == typeof(ushort)
		|| typeof(T) == typeof(byte);

	public bool IsSigned
		=> typeof(T) == typeof(int)
		|| typeof(T) == typeof(long)
		|| typeof(T) == typeof(short)
		|| typeof(T) == typeof(sbyte);

	public bool IsFloat
		=> typeof(T) == typeof(float)
		|| typeof(T) == typeof(double)
		|| typeof(T) == typeof(Half);
}