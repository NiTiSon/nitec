using System;
using System.Diagnostics;
using System.IO;
using CommunityToolkit.Diagnostics;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.Compilation;
using NiteCompiler.Compiler;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.Metadata;

internal sealed class MetadataLibraryBuilder : SymbolVisitor<MetadataEntry?, MetadataEntry?>
{
	internal const int FormatVersion = 1;

	private readonly NiteCompilation _compilation;
	private readonly Table?[] _tables;
	private readonly StringTable _stringTable;
	private uint _libraryNameId;

	private MetadataLibraryBuilder(NiteCompilation compilation)
	{
		_compilation = compilation;
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

	public static bool Translate(NiteCompilation compilation, Stream library, BindingDiagnosticBag diagnostics)
	{
		Guard.CanWrite(library);

		MetadataLibraryBuilder builder = new(compilation);
		builder.Visit(compilation.SourceLibrary, null);
		FunctionCompiler.CompileBodies(compilation, diagnostics, builder);

		using BinaryWriter writer = new(library);
		writer.Write(['n', 'l', 'i', 'b']);
		writer.Write((ushort)FormatVersion);
		writer.Write(builder._libraryNameId);
		Span<byte> reserved = stackalloc byte[6 + 16];
		writer.Write(reserved);
		builder._stringTable.Write(writer);
		foreach (Table? table in builder._tables)
		{
			table?.Write(writer);
		}

		return diagnostics.Diagnostics.HasAnyErrors;
	}

	public override MetadataEntry? VisitLibrary(LibrarySymbol lib, MetadataEntry? container)
	{
		_libraryNameId = _stringTable.AddOrGet(lib.Name);
		return lib.GlobalModule.Accept(this, null);
	}

	public override MetadataEntry VisitModule(ModuleSymbol symbol, MetadataEntry? library)
	{
		uint nameId = _stringTable.AddOrGet(symbol.IsGlobalModule ? symbol.Name : symbol.ToDisplayString());

		MetadataEntry module;
		if (library == null)
		{
			module = GetTable(MetadataKind.ModuleDeclaration).Add((id) => new ModuleDeclarationMetadata(id, symbol, nameId));
		}
		else
		{
			module = GetTable(MetadataKind.ModuleReference).Add((id) => new ModuleReferenceMetadata(id, symbol, library.Id, nameId));
		}

		foreach (Symbol member in symbol.GetMembers())
		{
			if (member.Kind == SymbolKind.Module)
			{
				Visit(member, library);
			}
			else
			{
				Visit(member, module);
			}
		}

		return module;
	}

	public override MetadataEntry VisitType(TypeSymbol symbol, MetadataEntry? container)
	{
		Debug.Assert(container != null);
		uint nameId = _stringTable.AddOrGet(symbol.Name);

		MetadataEntry type = GetTable(MetadataKind.TypeDeclaration).Add((id) => new TypeDeclarationMetadata(id, symbol, container.Id, nameId));

		foreach (Symbol member in symbol.GetMembers())
		{
			Visit(member, type);
		}

		return type;
	}

	public override MetadataEntry VisitFunction(FunctionSymbol symbol, MetadataEntry? container)
	{
		Debug.Assert(container != null);
		uint nameId = _stringTable.AddOrGet(symbol.Name);

		MetadataEntry function =
			GetTable(MetadataKind.FunctionDeclaration).Add((id) => new FunctionDeclarationMetadata(id, symbol, container.Id, nameId));

		return function;
	}

	public void SetFunctionBody(FunctionSymbol function, FunctionBody emittedBody)
	{
		Debug.Assert(function is SourceFunctionSymbol);
		(GetTable(MetadataKind.FunctionDeclaration).Get(function) as FunctionDeclarationMetadata)!.Body = emittedBody;
	}
}