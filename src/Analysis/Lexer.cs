using System;
using NiteCompiler.Analysis.Tokens;

namespace NiteCompiler.Analysis;

public sealed class Lexer
{
	private readonly string content;
	private uint pos, line, column;

	public Lexer(string content)
	{
		this.content = content;
		pos = 0;
		line = column = 1;
	}

	private char this[uint index]
		=> content[(int)index];

	public bool IsEOF
		=> pos >= content.Length;
	private TextAnchor Position
		=> new(pos, line, column);


	private char c;
	public Token Next()
	{
		if (IsEOF)
			return new EOFToken(Position);

		c = this[pos];
	
	SKIP_WHITESPACE:
		uint begin = pos;
		TextAnchor beginPosition = Position;

		if (c is '#')
			throw new Exception("Directives not allowed yet!");
		
		#region Space
		if (IsSpace(c))
		{
			MoveNext();
			while(IsSpace(c))
				MoveNext();

			goto SKIP_WHITESPACE;
		}
		else if (IsNewLine(c))
		{
			MoveNext();
			goto SKIP_WHITESPACE;
		}
		#endregion

		#region Identifiers
		if (IsCharBegin(c))
		{
			MoveNext();
			while(IsCharContinue(c))
			{
				MoveNext();
			}

			return new IdentifierToken(beginPosition, content.Substring((int)begin, (int)(pos - begin))); // Substring is rich operation; Not recommended to use it  
		}
		#endregion

		#region Literals
		if (IsDecDigit(c))
		{
			if (c is '0')
			{
				MoveNext();
				if (c is 'x' or 'X')
					goto HEX;
				else if (c is 'b' or 'B')
					goto BIN;
				else
					goto DEC; 
			}

			MoveNext();
		DEC:
			while(IsDecDigit(c))
				MoveNext();
			goto RET;
		
		BIN:
			MoveNext(); // Because of 0B
			while(IsBinDigit(c))
				MoveNext();
			goto RET;
		
		HEX:
			MoveNext(); // Because of 0X
			while(IsHexDigit(c))
				MoveNext();
			goto RET;

		RET:
			return new IntegerToken(beginPosition, content.Substring((int)begin, (int)(pos - begin)));
		}
		else if (IsDecDigit(c)) // 01231232132131291938712
		{
			MoveNext();
			while (IsDecDigit(c) || c is '_')
				MoveNext();

			return new IntegerToken(beginPosition, content.Substring((int)begin, (int)(pos - begin)));
		}
		
		#endregion

		MoveNext();
		return new UnexpectedToken(Position);
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
		if (pos >= content.Length - 1)
		{
			#if DEBUG
			System.Console.Error.WriteLine($"Stream is end: {pos}");
			#endif
			return false;
		}

		char next = this[++pos];
		if (next is '\n')
		{
			line++;
			column = 1;
		}
		else if (next is not '\r')
		{
			column++;
		}

		c = next;
		return true;
	}
}