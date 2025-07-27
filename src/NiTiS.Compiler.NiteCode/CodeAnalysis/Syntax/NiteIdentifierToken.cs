using System.Collections.Immutable;
using NiTiS.Compiler.CodeAnalysis.Syntax;
using NiTiS.Compiler.CodeAnalysis.Text;

namespace NiTiS.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class NiteIdentifierToken : NiteToken
{
	public string Identifier { get; }
	public NiteIdentifierToken(string identifier, TextSpan span, ImmutableArray<Trivia> leadingTrivia, ImmutableArray<Trivia> trailingTrivia)
		: base(SyntaxKind.Identifier, SyntaxKind.Identifier, span, leadingTrivia, trailingTrivia)
	{
		Identifier = identifier;
	}

	public NiteIdentifierToken(SyntaxKind contextualKind, string identifier, TextSpan span, ImmutableArray<Trivia> leadingTrivia, ImmutableArray<Trivia> trailingTrivia)
		: base(SyntaxKind.Identifier, contextualKind, span, leadingTrivia, trailingTrivia)
	{
		Identifier = identifier;
	}

	public bool IsEscaped => Identifier[0] == '`';

	public override string ToString()
	{
		if (this.ContextualKind.IsContextual)
		{
			return $"Token {this.Kind}/{this.ContextualKind} {Span} = {Identifier}";
		}
		return $"Token {this.Kind} {Span} = {Identifier}";
	}
}