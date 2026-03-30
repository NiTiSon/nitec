using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceParameterSymbol : ParameterSymbol
{
	public override string Name { get; }
	public override int Ordinal { get; }
	public override TypeSymbol Type { get; }
	public override Symbol ContainingSymbol { get; }

	private SourceParameterSymbol(Symbol owner, int ordinal, TypeSymbol parameterType, string name, Location nameLocation, SyntaxReference reference)
	{
		ContainingSymbol = owner;
		Ordinal = ordinal;
		Type = parameterType;
		Name = name;
	}

	public static SourceParameterSymbol Create(Binder context, Symbol owner, TypeSymbol parameterType,
		ParameterSyntax syntax, int ordinal, BindingDiagnosticBag diagnostics)
	{
		string name = syntax.Name.GetName();

		Location location = Location.Create(syntax.Name);

		return new SourceParameterSymbol(owner, ordinal, parameterType, name, location, syntax.CreateReference());
	}
}