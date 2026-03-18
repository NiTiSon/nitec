using System;
using System.IO;
using CommunityToolkit.Diagnostics;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.Compilation;

namespace NiteCompiler.Metadata;

internal sealed class MetadataLibraryBuilder : SymbolVisitor<MetadataEntry?, MetadataEntry?>
{
	private readonly Table?[] _tables;
	private readonly StringTable _stringTable;

	private MetadataLibraryBuilder()
	{
		_tables = new Table?[MetadataKind.Count];
		_stringTable = new();
	}

	private Table GetTable(MetadataKind kind)
	{
		ref Table? table = ref _tables[(int)kind - 1];

		if (table == null)
		{
			table = new Table(kind);
		}

		return table;
	}

	public static object Translate(NiteCompilation compilation, Stream library)
	{
		Guard.CanWrite(library);
		// Library:
		// + name
		// + dependencies
		// + string table
		// + tables
		MetadataLibraryBuilder builder = new();
		builder.Visit(compilation.SourceLibrary, null);

		throw new NotImplementedException();
	}

	public override MetadataEntry? VisitLibrary(LibrarySymbol lib, MetadataEntry? container)
	{
		_stringTable.AddOrGet(lib.Name);
		return lib.GlobalModule.Accept(this, null);
	}

	public override MetadataEntry VisitModule(ModuleSymbol symbol, MetadataEntry? library)
	{
		uint nameId = _stringTable.AddOrGet(symbol.IsGlobalModule ? symbol.Name : symbol.ToDisplayString());

		MetadataEntry module;
		if (library == null)
		{
			module = GetTable(MetadataKind.ModuleDeclaration).Add((id) => new ModuleDeclarationMetadata(id, nameId));
		}
		else
		{
			module = GetTable(MetadataKind.ModuleReference).Add((id) => new ModuleReferenceMetadata(id, library.Id, nameId));
		}

		foreach (Symbol member in symbol.GetMembers())
		{
			Visit(member, module);
		}

		return module;
	}

	public override MetadataEntry VisitType(TypeSymbol symbol, MetadataEntry? container)
	{
		uint nameId = _stringTable.AddOrGet(symbol.Name);

		MetadataEntry type = GetTable(MetadataKind.TypeDeclaration).Add((id) => new TypeDeclarationMetadata(id, container!.Id, nameId));

		foreach (Symbol member in symbol.GetMembers())
		{
			Visit(member, type);
		}

		return type;
	}
}