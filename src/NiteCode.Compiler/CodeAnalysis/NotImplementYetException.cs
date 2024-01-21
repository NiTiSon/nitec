using System;
using System.Runtime.CompilerServices;

namespace NiteCode.Compiler.CodeAnalysis;

internal sealed class NotImplementYetException : NotImplementedException
{
	public int LineNumber { get; }
	public string Caller { get; }
	public string FilePath { get; }


	public NotImplementYetException([CallerLineNumber] int lineNumber = 0, [CallerMemberName] string? caller = null, [CallerFilePath] string? callerFilePath = null)
	{
		LineNumber = lineNumber;
		Caller = caller!;
		FilePath = callerFilePath!;
	}

	public override string Message => $"Method {Caller} is not implement yet; location: {FilePath}:L{LineNumber}";
}