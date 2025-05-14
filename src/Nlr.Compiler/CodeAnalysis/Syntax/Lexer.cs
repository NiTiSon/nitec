using System.Collections.Immutable;
using Nlr.Compiler.CodeAnalysis.Text;
using Nlr.Compiler.Diagnostics;

namespace Nlr.Compiler.CodeAnalysis.Syntax;

public abstract class Lexer
{
	protected readonly Source _source;
	protected readonly Window _window;
	
	protected readonly ImmutableArray<Trivia>.Builder _triviaBuilder;

	protected DiagnosticBag Diagnostics { get; }
	
	protected Lexer(DiagnosticBag diagnostics, Source source)
	{
		Diagnostics = diagnostics;
		_source = source;
		_window = new(source);
		_triviaBuilder = ImmutableArray.CreateBuilder<Trivia>();
	}

	public abstract Token Lex();
}