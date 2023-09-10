using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
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
		uint begin = pos;
		TextAnchor beginPosition = Position;

		if (c is '#')
			throw new Exception("Directives not allowed yet!");
		
		if (IsCharBegin(c))
		{
			MoveNext();
			while(IsCharContinue(c))
			{
				MoveNext();
			}

			return new IdentifierToken(beginPosition, content.Substring((int)begin, (int)(pos - begin))); // Substring is rich operation; Not recommended to use it  
		}

		MoveNext();
		return new UnexpectedToken(Position);
	}

	private bool IsCharBegin(char c) // Currently allowed only ascii characters - TODO: Add support for world-wide chars
		=> c >= 'a' && c <= 'z'
		|| c >= 'A' && c <= 'Z'
		|| c is '_' or '@' // '@' symbol ignored by compiler, but not ignored by lexer and parser; `public static usize @sizeof()` compiles into `sizeof(): usize ` without any '@' at beginning
		;

	
	private bool IsCharContinue(char c) // Currently allowed only ascii characters and decimal numbers - TODO: Add support for world-wide chars
		=> c >= 'a' && c <= 'z'
		|| c >= 'A' && c <= 'Z'
		|| c >= '0' && c <= '9'
		|| c is '_'
		;

	private bool MoveNext()
	{
		if (pos >= content.Length - 1)
		{
			System.Console.Error.WriteLine($"Stream is end: {pos}");
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