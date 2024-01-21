using System;

namespace NiteCode.Compiler.CodeAnalysis;

internal sealed class ParsingNotEndedYetException : Exception
{
	public override string Message => "The operation has been called before parsing is ended.";
}