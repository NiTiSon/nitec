using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceSelfParameterSymbol : SelfParameterSymbol
{
	public override TypeSymbol Type { get; }
	public override SourceConstructorSymbol ContainingSymbol { get; }
	public override TypeSymbol ContainingType { get; }

	private SourceSelfParameterSymbol(TypeSymbol containingType, SourceConstructorSymbol constructor, TypeSymbol type)
	{
		Type = type;
		ContainingSymbol = constructor;
		ContainingType = containingType;
	}


	public static SourceSelfParameterSymbol Create(Binder context, SourceConstructorSymbol owner, BindingDiagnosticBag diagnostics)
	{
		TypeSymbol? ownerType = owner.ContainingType;

		ownerType ??= context.CreateErrorType("self");

		TypeSymbol selfType = owner.DeclaringCompilation!.CreateReferenceType(ownerType, true, false);

		return new SourceSelfParameterSymbol(ownerType, owner, selfType);
	}
}