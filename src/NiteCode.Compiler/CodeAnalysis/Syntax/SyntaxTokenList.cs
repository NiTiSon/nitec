using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

[CollectionBuilder(typeof(SyntaxTokenList), methodName: "Create")]
public readonly struct SyntaxTokenList
{
	private readonly ImmutableArray<SyntaxToken> tokens;

	internal SyntaxTokenList(ImmutableArray<SyntaxToken> tokens)
	{
		this.tokens = tokens;
	}

	public SyntaxToken this[uint index]
	{
		get
		{
			if (tokens.Length <= index)
				throw new ArgumentOutOfRangeException(nameof(index));

			return tokens[(int)index];
		}
	}

	public uint Count => (uint)tokens.Length;

	public static SyntaxTokenList Empty => default;

	public static SyntaxTokenList Create(ReadOnlySpan<SyntaxToken> tokens)
	{
		if (tokens.Length == 0)
			return default;

		return new(tokens: tokens.ToImmutableArray());
	}

	public static SyntaxTokenList Create(SyntaxToken token)
	{
		return new(tokens: [token]);
	}

	public static SyntaxTokenList Create()
	{
		return default;
	}
}