using System.Collections.Immutable;
using NiTiS.Compiler.CodeAnalysis.Text;
using NiTiS.Compiler.Diagnostics;

namespace NiTiS.Compiler.CodeAnalysis.Syntax;

public abstract class Lexer
{
	protected readonly SourceText SourceText;
	protected readonly SlidingWindow Window;

	protected readonly ImmutableArray<Trivia>.Builder TriviaBuilder;

	protected DiagnosticBag Diagnostics { get; }

	protected Lexer(DiagnosticBag diagnostics, SourceText sourceText)
	{
		Diagnostics = diagnostics;
		SourceText = sourceText;
		Window = new(sourceText);
		TriviaBuilder = ImmutableArray.CreateBuilder<Trivia>();
	}

	public abstract Token Lex();
}