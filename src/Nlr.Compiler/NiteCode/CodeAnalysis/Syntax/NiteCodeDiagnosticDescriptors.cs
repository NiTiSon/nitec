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
		UnexpectedTokenAny,
		IncompleteMember,
		ExceptedExpression
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
		diagnostics.Add(UnexpectedTokenAny, providedToken, expectedKind);
	}

	public static void ReportIncompleteMember(this DiagnosticBag diagnostics, IncompleteMemberSyntax member)
	{
		diagnostics.Add(IncompleteMember, member);
	}

	public static void ReportExceptedExpression(this DiagnosticBag diagnostics)
	{
		diagnostics.Add(ExceptedExpression);
	}

	// ReSharper disable FormatStringProblem
	static NiteCodeDiagnosticDescriptors()
	{
		BadToken = new("bad_token", "Bad token '{0}' is presented.");
		NotTerminatedMultiLineComment = new("not_terminated_multiline_comment", "Not terminated multiline comment.");
		UnexpectedToken = new("unexpected_token", "Required '{1}' token, but unexpected token '{0}' is presented.");
		UnexpectedTokenAny = new("unexpected_token", "Required any of {1} token, but unexpected token '{0}' is presented.");
		IncompleteMember = new("incomplete_member", "Member is not complete.");
		ExceptedExpression = new("excepted_expression", "The compiler suggests that there should be an expression.");
	}
}