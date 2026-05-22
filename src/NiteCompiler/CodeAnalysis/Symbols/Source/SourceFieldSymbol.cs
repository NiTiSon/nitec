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

	public override bool IsStatic
	{
		get
		{
			if (ContainingSymbol is ModuleSymbol)
			{
				return true;
			}

			return _flags.HasFlag(Flags.IsStatic);
		}
	}

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

	private readonly Flags _flags;
	public SourceFieldSymbol(SourceNamedTypeSymbol containingType, FieldDeclarationSyntax syntax)
	{
		_containingType = containingType;
		_syntax = syntax;

		BindingDiagnosticBag diagnostics = BindingDiagnosticBag.GetInstance();
		foreach (Token modifier in syntax.Modifiers)
		{
			if (modifier.TKind == TokenKind.Static)
			{
				if (!SetFlag(ref _flags, Flags.IsStatic))
				{
					diagnostics.Diagnostics.ReportDuplicateModifier(modifier.Location, modifier.TKind.ToString());
				}
			}
			else
			{
				// TODO: report wrong modifier
			}
		}

		AddDeclarationDiagnostics(diagnostics);
	}

	[Flags]
	private enum Flags : ushort
	{
		IsStatic = 1 << 0,
	}

	/// <returns><see langword="true"/> when flag is set; otherwise <see langword="false"/>.</returns>
	private static bool SetFlag(ref Flags modifiers, Flags flag)
	{
		Flags previous = modifiers;

		modifiers |= flag;
		return previous != modifiers;
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
