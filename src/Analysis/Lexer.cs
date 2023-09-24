using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Primitives;
using NiteCompiler.Analysis.Text;
using NiteCompiler.Analysis.Tokens;

namespace NiteCompiler.Analysis;

public sealed class Lexer
{
	/// <summary>
	/// The source text file consumed for tokenization.
	/// </summary>
	private readonly CodeSource source;
	private readonly LanguageOptions options;
	private readonly DiagnosticBag diagnostics;
	private uint pos, line, column;

	public Lexer(LanguageOptions options, CodeSource source, DiagnosticBag diagnostics)
	{
		this.options = options;
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

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private bool IsDecDigit(char c)
		=> c >= '0' && c <= '9';

	
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private bool IsBinDigit(char c)
		=> c is '0' or '1';

	
	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private bool IsHexDigit(char c)
		=> c >= '0' && c <= '9'
		|| c >= 'a' && c <= 'f'
		|| c >= 'A' && c <= 'F'
		;

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private bool IsCharBegin(char c) // Currently allowed only ascii characters - TODO: Add support for world-wide chars
		=> char.GetUnicodeCategory(c)
			is UnicodeCategory.UppercaseLetter
			or UnicodeCategory.LowercaseLetter
		|| c is '_' or '@' // '@' symbol ignored by compiler, but not ignored by lexer and parser; `public static usize @sizeof()` compiles into `sizeof(): usize ` without any '@' at beginning
		;

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private bool IsCharContinue(char c) // Currently allowed only ascii characters and decimal numbers - TODO: Add support for world-wide chars
		=> char.GetUnicodeCategory(c)
			is UnicodeCategory.UppercaseLetter
			or UnicodeCategory.LowercaseLetter
			or UnicodeCategory.DecimalDigitNumber
		|| c is '_'
		;

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private bool IsSpace(char c)
		=> char.GetUnicodeCategory(c)
			is UnicodeCategory.SpaceSeparator
		;

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private bool IsNewLine(char c)
		=> c is '\n'
		;

	private SyntaxKind GetKind(StringSegment segment)
	{
		if (segment[0] is '@') // Fast exit for @identifier
			return SyntaxKind.Identifier;

		if (segment == Names.Char)
			return SyntaxKind.KwChar;
		else if (segment == Names.Char7)
			return SyntaxKind.KwChar7;
		else if (segment == Names.Char8)
			return SyntaxKind.KwChar8;
		else if (segment == Names.Char16)
			return SyntaxKind.KwChar16;
		else if (segment == Names.Char32)
			return SyntaxKind.KwChar32;
		else if (segment == Names.I8)
			return SyntaxKind.KwI8;
		else if (segment == Names.I16)
			return SyntaxKind.KwI16;
		else if (segment == Names.I32)
			return SyntaxKind.KwI32;
		else if (segment == Names.I64)
			return SyntaxKind.KwI64;
		//else if (segment == Names.I128)
		//	return SyntaxKind.KwI128;
		else if (segment == Names.U8)
			return SyntaxKind.KwU8;
		else if (segment == Names.U16)
			return SyntaxKind.KwU16;
		else if (segment == Names.U32)
			return SyntaxKind.KwU32;
		else if (segment == Names.U64)
			return SyntaxKind.KwU64;
		//else if (segment == Names.U128)
		//	return SyntaxKind.KwU128;
		else if (segment == Names.F16)
			return SyntaxKind.KwF16;
		else if (segment == Names.F32)
			return SyntaxKind.KwF32;
		else if (segment == Names.F64)
			return SyntaxKind.KwF64;
		else if (segment == Names.Void)
			return SyntaxKind.KwVoid;
		else if (segment == Names.Nil)
			return SyntaxKind.KwNil;
		else if (segment == Names.SelfInst)
			return SyntaxKind.KwSelf;
		else if (segment == Names.SelfType)
			return SyntaxKind.KwSelfType;
		else if (segment == Names.SelfType)
			return SyntaxKind.KwSelfType;

		return SyntaxKind.Identifier;
	}
	
	
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