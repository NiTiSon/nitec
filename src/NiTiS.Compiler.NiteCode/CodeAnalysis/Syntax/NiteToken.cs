using System.Collections.Immutable;
using NiTiS.Compiler.CodeAnalysis.Syntax;
using NiTiS.Compiler.CodeAnalysis.Text;

namespace NiTiS.Compiler.NiteCode.CodeAnalysis.Syntax;

public class NiteToken : Token
{
	public NiteToken(SyntaxKind kind, TextSpan span, ImmutableArray<Trivia> leadingTrivia, ImmutableArray<Trivia> trailingTrivia) : base((uint)kind, span, leadingTrivia, trailingTrivia) {}

	public NiteToken(SyntaxKind kind, SyntaxKind contextualKind, TextSpan span, ImmutableArray<Trivia> leadingTrivia, ImmutableArray<Trivia> trailingTrivia) : base((uint)kind, (uint)contextualKind, span, leadingTrivia, trailingTrivia) {}

	public override string ToString()
	{
		string? value = SyntaxFacts.GetText(this.Kind);

		return $"Token {this.Kind} {Span} {(value is not null ? ("= " + value) : string.Empty)}";
	}
}