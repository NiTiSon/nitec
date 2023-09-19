using System;
using NiteCompiler.Analysis.Text;
using NiteCompiler.Analysis.Tokens;

namespace NiteCompiler.Analysis;

public sealed class Lexer
{
	/// <summary>
	/// The source text file consumed for tokenization.
	/// </summary>
	private readonly CodeSource source;
	private readonly DiagnosticBag diagnostics;
	private uint pos, line, column;

	public Lexer(CodeSource source, DiagnosticBag diagnostics)
	{
		this.source = source;
		this.diagnostics = diagnostics;
		pos = 0;
		line = column = 1;
	}

	public bool IsEOF
		=> pos >= source.Content.Length;
	private TextAnchor Position
		=> new(pos, line, column);

	private char c;
	public Token NextToken()
	{
		if (IsEOF)
			return new EOFToken(Position);


		MoveNext();
		return new BadToken(Position);
	}

	private bool IsCharBegin(char c) // Currently allowed only ascii characters - TODO: Add support for world-wide chars
		=> c >= 'a' && c <= 'z'
		|| c >= 'A' && c <= 'Z'
		|| c is '_' or '@' // '@' symbol ignored by compiler, but not ignored by lexer and parser; `public static usize @sizeof()` compiles into `sizeof(): usize ` without any '@' at beginning
		;

	private bool IsDecDigit(char c)
		=> c >= '0' && c <= '9';

	
	private bool IsBinDigit(char c)
		=> c is '0' or '1';

	
	private bool IsHexDigit(char c)
		=> c >= '0' && c <= '9'
		|| c >= 'a' && c <= 'f'
		|| c >= 'A' && c <= 'F'
		;
	
	private bool IsCharContinue(char c) // Currently allowed only ascii characters and decimal numbers - TODO: Add support for world-wide chars
		=> c >= 'a' && c <= 'z'
		|| c >= 'A' && c <= 'Z'
		|| c >= '0' && c <= '9'
		|| c is '_'
		;

	private bool IsSpace(char c)
		=> c is ' ' or '\t' or '\r'
		;

	private bool IsNewLine(char c)
		=> c is '\n'
		;

	private bool MoveNext()
	{
		if (pos >= source.Content.Length - 1)
		{
			#if DEBUG
			pos = (uint)source.Content.Length;
			System.Console.Error.WriteLine($"Stream is end: {pos}");
			#endif
			return false;
		}

		char next = source.Content[(int)++pos];
		if (next is '\n')
		{
			line++;
			column = 0; // Works for some reason
		}
		else if (next is not '\r')
		{
			column++;
		}

		c = next;
		return true;
	}
}