using NiteCompiler.Analysis.Text;

namespace NiteCompiler.Analysis;

public sealed class Diagnostic
{
	public readonly bool IsError;
	public readonly CodeSource Source;
	public readonly TextAnchor Location;
	public readonly string Code;
	public readonly string Message;

	public Diagnostic(bool error, CodeSource source, TextAnchor location, string message)
	{
		IsError = error;
		Source = source;
		Location = location;
		Code = IsError ? "ERR" : "WRN";
		Message = message;
	}

	public Diagnostic(bool error, CodeSource source, TextAnchor location, string code, string message)
	{
		IsError = error;
		Source = source;
		Location = location;
		Code = code;
		Message = message;
	}
}