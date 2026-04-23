using System.Collections.Immutable;
using System.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class ModuleSymbol : ContainerSymbol
{
	public sealed override SymbolKind Kind => SymbolKind.Module;

	public bool IsGlobalModule => ContainingSymbol is not ModuleSymbol;

	public override string ToDisplayString(SymbolFormat format = SymbolFormat.Default)
	{
		Debug.Assert(format.IsValid);

		bool includeLibrary = format.HasFlag(SymbolFormat.IncludeLibrary);
		bool emitGlobal = format.HasFlag(SymbolFormat.EmitGlobalModule);
		bool omitPath = format.HasFlag(SymbolFormat.OmitModulePath);

		if (IsGlobalModule)
		{
			string? libraryPart = includeLibrary
				? ContainingLibrary?.ToDisplayString(format)
				: null;

			if (!emitGlobal)
			{
				return libraryPart ?? string.Empty;
			}

			return libraryPart != null ? $"{libraryPart}{Name}" : Name;
		}

		if (omitPath)
		{
			return Name;
		}

		if (ContainingSymbol is ModuleSymbol { IsGlobalModule: true } && !emitGlobal)
		{
			return Name;
		}

		return $"{ContainingSymbol!.ToDisplayString(format)}::{Name}";
	}

	public override void Accept(SymbolVisitor visitor)
	{
		visitor.VisitModule(this);
	}

	public override TResult? Accept<TResult>(SymbolVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitModule(this);
	}

	public override TResult? Accept<TResult, TArgument>(SymbolVisitor<TResult, TArgument> visitor, TArgument arg) where TResult : default
	{
		return visitor.VisitModule(this, arg);
	}

	public abstract ImmutableArray<Symbol> GetMembersUnordered();
}