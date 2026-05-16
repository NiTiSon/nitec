using System.Threading;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceConstructorParameterSymbol : ParameterSymbol
{
	private readonly ParameterSyntax _syntax;

	public override string Name => _syntax.Name.GetName();
	public override int Ordinal { get; }
	public override SourceConstructorSymbol ContainingSymbol { get; }

	public override TypeSymbol Type
	{
		get
		{
			if (field == null)
			{
				Interlocked.CompareExchange(ref field, MakeType(), null);
			}

			return field;
		}
	}

	public SourceConstructorParameterSymbol(SourceConstructorSymbol owner, ParameterSyntax syntax, int ordinal)
	{
		ContainingSymbol = owner;
		_syntax = syntax;
		Ordinal = ordinal;
	}

	private TypeSymbol MakeType()
	{
		BindingDiagnosticBag diagnostics = BindingDiagnosticBag.GetInstance();

		BinderFactory factory = ContainingSymbol.DeclaringCompilation!.GetBinderFactory(_syntax.Tree);
		Binder binder = factory.GetBinder(_syntax.TypeClauseSyntax);
		TypeSymbol result = binder.BindType(_syntax.TypeClauseSyntax.Type, diagnostics);

		if (!diagnostics.IsEmpty)
		{
			ContainingSymbol.DeclaringCompilation!.DeclarationDiagnostics.AddRange(diagnostics.Diagnostics);
		}

		diagnostics.Free();
		return result;
	}
}
