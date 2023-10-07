using System.Globalization;
using System.Runtime.CompilerServices;

namespace NiteCompiler.Analysis;
public partial class Lexer
{
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
		|| c is '\t'
		;
}