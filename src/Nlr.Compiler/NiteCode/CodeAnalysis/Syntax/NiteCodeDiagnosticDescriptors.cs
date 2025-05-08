using Nlr.Compiler.Diagnostics;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class NiteCodeDiagnosticDescriptors
{
	public static DiagnosticDescriptor
		BadToken,
		NotTerminatedMultilineComment,
		UnexpectedToken
		;

	// ReSharper disable FormatStringProblem
	static NiteCodeDiagnosticDescriptors()
	{
		BadToken = new("bad_token", "Bad token '{0}' is presented.");
		NotTerminatedMultilineComment = new("not_terminated_multiline_comment", "Not terminated multiline comment.");
		UnexpectedToken = new("unexpected_token", "Unexpected token '{0}' is presented.");
	}
}