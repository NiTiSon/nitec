using Microsoft.Extensions.Primitives;
using Nlr.Compiler.Text;
using System;
using System.Collections.Immutable;
using System.Globalization;

namespace Nlr.Compiler.CodeAnalysis.NiteCode;

public sealed class SyntaxTokenWithValue<T> : SyntaxToken
{
	private readonly T value;

	public SyntaxTokenWithValue(SyntaxKind kind, StringSegment text, TextSpan span, T value, ImmutableArray<SyntaxTrivia> leadingTrivia, ImmutableArray<SyntaxTrivia> trailingTrivia) : base(kind, text, span, leadingTrivia, trailingTrivia)
	{
		this.value = value;
	}

	public override string ToString()
	{
		if (value is IFormattable valueF)
		{
			return $"{{{Kind}, {Span}, {valueF.ToString(null, CultureInfo.InvariantCulture)}}}";
		}

		return $"{{{Kind}, {Span}, {value}}}";
	}
}
