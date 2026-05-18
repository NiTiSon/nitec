using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal class SourceParameterSymbol : ParameterSymbol
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

	public static SourceParameterSymbol CreateGenerative(Binder context, SourceConstructorSymbol owner,
		GenerativeParameterSyntax syntax, int ordinal, BindingDiagnosticBag diagnostics)
	{
		Location nameLocation = syntax.FieldName.Location;
		string fieldName = syntax.FieldName.GetName();

		// TODO: report when not resolved; [generative-field-is-not-resolved] or smth lk tht
		FieldSymbol? field = context.LookupFieldSymbolWithinType(owner.ContainingType, fieldName);

		field ??= context.CreateErrorField(fieldName);

		return new SourceGenerativeParameterSymbol(owner, ordinal, field, nameLocation, syntax.CreateReference());
	}

	internal sealed class SourceGenerativeParameterSymbol : SourceParameterSymbol
	{
		public FieldSymbol Field { get; }

		public SourceGenerativeParameterSymbol(SourceConstructorSymbol owner, int ordinal, FieldSymbol field,
			Location nameLocation, SyntaxReference reference)
			: base(owner, ordinal, field.Type, field.Name, nameLocation, reference)
		{
			Field = field;
		}
	}
}