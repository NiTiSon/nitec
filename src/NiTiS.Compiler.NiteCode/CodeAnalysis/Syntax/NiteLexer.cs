using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Wasm;
using NiTiS.Compiler.CodeAnalysis.Syntax;
using NiTiS.Compiler.CodeAnalysis.Text;
using NiTiS.Compiler.Diagnostics;

namespace NiTiS.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed partial class NiteLexer : Lexer
{
	public NiteLexer(DiagnosticBag diagnostics, SourceText sourceText) : base(diagnostics, sourceText) {}

	private struct TokenInfo
	{
		public SyntaxKind Kind;
		public SyntaxKind ContextualKind;
		public TextSpan Span;
		public string Value;
		public PackedNumeric Number;
		public NumericLiteralType NumberType;
	}

	public override Token Lex()
	{
		ReadTrivia(leading: true);
		ImmutableArray<Trivia> leading = TriviaBuilder.ToImmutable();

		Unsafe.SkipInit(out TokenInfo info);
		ReadToken(ref info);
		info.Span = Window.LexemeSpan;

		ReadTrivia(leading: false);
		ImmutableArray<Trivia> trailing = TriviaBuilder.ToImmutable();

		if (info.ContextualKind.IsContextual)
		{
			return new NiteToken(info.Kind, info.ContextualKind, info.Span, leading, trailing);
		}
		else
		{
			return new NiteToken(info.Kind, info.Span, leading, trailing);
		}
	}

	private void ReadToken(ref TokenInfo info)
	{
		if (Window.IsAtTheEnd)
		{
			info.Kind = SyntaxKind.EndOfFile;
			return;
		}

		char c = Window.Current;

		if (char.IsAsciiLetter(c))
		{
			// Read identifier
		}

		switch (c)
		{

		}
	}
}