using Nlr.Compiler.Diagnostics;
using Nlr.Compiler.Text;

namespace Nlr.Compiler.CodeAnalysis;

public abstract class Lexer
{
	protected readonly TextWindow window;
	protected readonly SourceText text;

	public DiagnosticBag Diagnostics { get; }

	public Lexer(SourceText source, DiagnosticBag diagnostics)
	{
		text = source;
		this.Diagnostics = diagnostics;
		window = new TextWindow(source);
	}

	protected void ReportAnonymous(DiagnosticDescriptor descriptor)
	{
		Diagnostics.Report(descriptor);
	}

	protected void Report(DiagnosticDescriptor descriptor, TextSpan span)
	{
		Diagnostics.Report(descriptor, text, span);
	}
}