using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using CommunityToolkit.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class ModuleSymbol : Symbol, IEnumerable<Symbol>
{
	private readonly List<Symbol> _members = [];

	public override string Name { get; }
	public bool IsCombined { get; }

	public IEnumerable<Symbol> Members => _members;

	public IEnumerable<TypeSymbol> Types => Members.OfType<TypeSymbol>();
	public IEnumerable<FunctionSymbol> Functions => Members.OfType<FunctionSymbol>();
	// public IEnumerable<FieldSymbol> Fields => Members.OfType<FieldSymbol>();

	public bool IsGlobalModule => Name == WellKnownSemantic.GlobalModuleName;

	public ModuleSymbol(string name)
	{
		Name = name;
		IsCombined = false;
	}

	private ModuleSymbol(string name, bool isCombined)
	{
		Name = name;
		IsCombined = isCombined;
	}

	public static ModuleSymbol Combine(params IEnumerable<ModuleSymbol> modules)
	{
		ModuleSymbol? combined = null;

		foreach (ModuleSymbol module in modules)
		{
			combined ??= new ModuleSymbol(module.Name, isCombined: true);

			if (module.Name != combined.Name)
			{
				ThrowHelper.ThrowArgumentException("One of modules have other name.", nameof(modules));
			}

			combined.AcceptMembersFromModule(module);
		}

		if (combined == null)
		{
			ThrowHelper.ThrowArgumentException("Modules argument cannot be empty", nameof(modules));
		}

		return combined;
	}

	public override Symbol? ContainingSymbol => ContainingLibrary;

	[MemberNotNullWhen(false, nameof(IsCombined))]
	public LibrarySymbol? ContainingLibrary { get; internal init; }

	public override SymbolKind Kind => SymbolKind.Module;

	public void AddMember(Symbol symbol)
	{
		_members.Add(symbol);
	}

	private void AcceptMembersFromModule(ModuleSymbol module)
	{
		foreach (var member in module.Members)
		{
			AddMember(member);
		}
	}

	public IEnumerator<Symbol> GetEnumerator()
	{
		return _members.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)_members).GetEnumerator();
	}
}