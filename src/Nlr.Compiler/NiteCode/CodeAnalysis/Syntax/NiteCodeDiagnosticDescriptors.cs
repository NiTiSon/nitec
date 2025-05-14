using Nlr.Compiler.CodeAnalysis.Syntax;
using Nlr.Compiler.CodeAnalysis.Text;
using Nlr.Compiler.Diagnostics;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public static class NiteCodeDiagnosticDescriptors
{
	public static DiagnosticDescriptor
		BadToken,
		NotTerminatedMultiLineComment,
		UnexpectedToken,
		UnexpectedTokenAny
		;

	public static void ReportBadToken(this DiagnosticBag diagnostics, TextSpan span)
	{
		diagnostics.Add(BadToken, span);
	}

	public static void ReportNotTerminatedMultiLineComment(this DiagnosticBag diagnostics, TextSpan commentOpen)
	{
		diagnostics.Add(NotTerminatedMultiLineComment, commentOpen);
	}
	
	public static void ReportUnexpectedToken(this DiagnosticBag diagnostics, Token providedToken, SyntaxKind expectedKind)
	{
		diagnostics.Add(UnexpectedToken, providedToken, expectedKind);
	}
	
	public static void ReportUnexpectedToken(this DiagnosticBag diagnostics, Token providedToken, SyntaxKind[] expectedKind)
	{
		diagnostics.Add(UnexpectedToken, providedToken, expectedKind);
	}

	// ReSharper disable FormatStringProblem
	static NiteCodeDiagnosticDescriptors()
	{
		BadToken = new("bad_token", "Bad token '{0}' is presented.");
		NotTerminatedMultiLineComment = new("not_terminated_multiline_comment", "Not terminated multiline comment.");
		UnexpectedToken = new("unexpected_token", "Required '{1}' token, but unexpected token '{0}' is presented.");
		UnexpectedTokenAny = new("unexpected_token", "Required any of {1} token, but unexpected token '{0}' is presented.");
	}
}