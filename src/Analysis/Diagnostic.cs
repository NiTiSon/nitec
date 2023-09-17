using NiteCompiler.Analysis.Text;

namespace NiteCompiler.Analysis;

public sealed class Diagnostic
{
	public readonly bool IsError;
	public readonly TextAnchor Location;
	public readonly string? Code;
	public readonly string Message;

	public Diagnostic(bool error, TextAnchor location, string message)
	{
		IsError = error;
		Location = location;
		Message = message;
	}

	public Diagnostic(bool error, TextAnchor location, string code, string message)
	{
		IsError = error;
		Location = location;
		Code = code;
		Message = message;
	}
}