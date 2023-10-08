using NiteCompiler.Analysis.Syntax;
using NiteCompiler.Analysis.Syntax.AST;
using NiteCompiler.Analysis.Syntax.Tokens;
using NiteCompiler.Analysis.Text;

namespace NiteCompiler.Analysis;

public sealed class Parser
{
	private readonly TokenStream stream;
	private readonly CodeSource source;
	private readonly DiagnosticBag diagnostics;
	private Token c;

	public Parser(TokenStream stream, CodeSource source, DiagnosticBag diagnostics)
	{
		this.stream = stream;
		this.source = source;
		this.diagnostics = diagnostics;
		MoveNext();
	}

	public CompilationUnit ParseCompilationUnit()
	{
		return null!;
	}

	public Identifier? ParseIdentifier()
	{
		if (c.Kind == SyntaxKind.Identifier)
		{
			diagnostics.ReportError(source, c.Location, ErrorCodes.ExpectedIdentifier, "_NOMSG_");
			return null!;
		}
		else
		{

			MoveNext();
		}
	}

	private bool MoveNext()
	{
		if (stream.IsEOF)
			return false;

		c = stream.NextToken();
		return true;
	}
}
