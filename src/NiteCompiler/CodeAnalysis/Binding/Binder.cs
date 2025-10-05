using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class Binder
{
	public DiagnosticBag Diagnostics { get; } = [];
	public Binder(Compilation compilation)
	{
	}
}