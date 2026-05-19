using System;
using System.Threading;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceFieldSymbol : FieldSymbol
{
	private readonly SourceNamedTypeSymbol _containingType;
	private readonly FieldDeclarationSyntax _syntax;

	public override Symbol ContainingSymbol => _containingType;
	public override string Name => _syntax.Name.GetName();

	private TypeSymbol? _lateinitType;
	public override TypeSymbol Type
	{
		get
		{
			if (_lateinitType == null)
			{
				Interlocked.CompareExchange(ref _lateinitType, MakeType(), null);
			}

			return _lateinitType;
		}
	}

	public SourceFieldSymbol(SourceNamedTypeSymbol containingType, FieldDeclarationSyntax syntax)
	{
		_containingType = containingType;
		_syntax = syntax;
	}

	private TypeSymbol MakeType()
	{
		BindingDiagnosticBag diagnostics = BindingDiagnosticBag.GetInstance();

		BinderFactory factory = DeclaringCompilation!.GetBinderFactory(_syntax.Tree);
		Binder binder = factory.GetBinder(_syntax.TypeClause);
		TypeSymbol result = binder.BindType(_syntax.TypeClause.Type, diagnostics);

		if (result.IsUnsized)
		{
			diagnostics.Diagnostics.ReportCannotUseUnsizedType(_syntax.TypeClause.Type.Location, result);
		}

		if (!diagnostics.IsEmpty)
		{
			AddDeclarationDiagnostics(diagnostics);
		}

		diagnostics.Free();
		return result;
	}

	internal override void ForceComplete(Predicate<Symbol>? filter, CancellationToken cancellationToken = default)
	{
		_ = Type;
	}
}
