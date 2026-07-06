using System;
using System.Collections.Immutable;
using System.Threading;
using NiteCompiler.Compilation;
using NiteCompiler.Metadata;

namespace NiteCompiler.CodeAnalysis.Symbols.Metadata;

internal sealed class MetadataLibrarySymbol : LibrarySymbol
{
	private readonly MetadataLibraryReader _reader;
	private readonly NiteCompilation _compilation;
	private readonly MetadataModuleSymbol?[] _modulesById;
	private readonly MetadataNamedTypeSymbol?[] _typesById;
	private MetadataModuleSymbol? _globalModule;

	public override string Name { get; }
	public override NiteCompilation DeclaringCompilation => _compilation;

	public override ModuleSymbol GlobalModule
	{
		get
		{
			if (_globalModule == null)
			{
				Interlocked.CompareExchange(ref _globalModule, FindGlobalModule(), null);
			}

			return _globalModule;
		}
	}

	public MetadataLibrarySymbol(NiteCompilation compilation, MetadataLibraryReader reader)
	{
		_compilation = compilation;
		_reader = reader;
		Name = reader.LibraryName;

		int moduleCount = reader.GetTableCount(MetadataKind.ModuleDeclaration);
		_modulesById = new MetadataModuleSymbol?[moduleCount + 1];

		for (uint i = 1; i <= moduleCount; i++)
		{
			ModuleDeclarationEntry entry = reader.GetModuleDeclaration(i);
			string moduleName = reader.GetString(entry.NameId);
			_modulesById[i] = new MetadataModuleSymbol(this, new MetadataId(MetadataKind.ModuleDeclaration, i), moduleName, entry);
		}

		int typeCount = reader.GetTableCount(MetadataKind.TypeDeclaration);
		_typesById = new MetadataNamedTypeSymbol?[typeCount + 1];

		for (uint i = 1; i <= typeCount; i++)
		{
			TypeDeclarationEntry entry = reader.GetTypeDeclaration(i);
			_typesById[i] = new MetadataNamedTypeSymbol(this, new MetadataId(MetadataKind.TypeDeclaration, i), entry);
		}
	}

	public string GetString(uint id) => _reader.GetString(id);

	public MetadataModuleSymbol? GetModuleById(uint index)
	{
		if (index < 1 || index >= (uint)_modulesById.Length)
		{
			return null;
		}

		return _modulesById[index];
	}

	public MetadataNamedTypeSymbol? GetTypeById(uint index)
	{
		if (index < 1 || index >= (uint)_typesById.Length)
		{
			return null;
		}

		return _typesById[index];
	}

	public MetadataNamedTypeSymbol? GetTypeByMetadataId(MetadataId id)
	{
		return GetTypeById(id.Value);
	}

	public ImmutableArray<MetadataNamedTypeSymbol> GetTypesByContainer(MetadataId containerId)
	{
		int count = _reader.GetTableCount(MetadataKind.TypeDeclaration);
		if (count == 0)
		{
			return [];
		}

		var builder = ImmutableArray.CreateBuilder<MetadataNamedTypeSymbol>();
		uint rawContainerValue = containerId.Value;
		for (uint i = 1; i <= count; i++)
		{
			TypeDeclarationEntry entry = _reader.GetTypeDeclaration(i);
			if (entry.ContainerId.Value == rawContainerValue)
			{
				builder.Add(_typesById[i]!);
			}
		}

		return builder.ToImmutable();
	}

	public ImmutableArray<MetadataFunctionSymbol> GetFunctionsByContainer(MetadataId containerId)
	{
		int count = _reader.GetTableCount(MetadataKind.FunctionDeclaration);
		if (count == 0)
		{
			return [];
		}

		var builder = ImmutableArray.CreateBuilder<MetadataFunctionSymbol>();
		uint rawContainerValue = containerId.Value;
		for (uint i = 1; i <= count; i++)
		{
			FunctionDeclarationEntry entry = _reader.GetFunctionDeclaration(i);
			if (entry.ContainerId.Value == rawContainerValue)
			{
				builder.Add(new MetadataFunctionSymbol(this, entry));
			}
		}

		return builder.ToImmutable();
	}

	public ImmutableArray<MetadataModuleSymbol> GetNestedModulesByContainer(MetadataId? containerId)
	{
		int count = _reader.GetTableCount(MetadataKind.ModuleDeclaration);
		if (count == 0)
		{
			return [];
		}

		var builder = ImmutableArray.CreateBuilder<MetadataModuleSymbol>();
		uint? rawContainerValue = containerId?.Value;
		for (uint i = 1; i <= count; i++)
		{
			ModuleDeclarationEntry entry = _reader.GetModuleDeclaration(i);
			bool matches = rawContainerValue.HasValue
				? entry.ContainerId.HasValue && entry.ContainerId.Value.Value == rawContainerValue.Value
				: !entry.ContainerId.HasValue;

			if (matches)
			{
				builder.Add(_modulesById[i]!);
			}
		}

		return builder.ToImmutable();
	}

	private MetadataModuleSymbol FindGlobalModule()
	{
		int count = _reader.GetTableCount(MetadataKind.ModuleDeclaration);
		for (uint i = 1; i <= count; i++)
		{
			ModuleDeclarationEntry entry = _reader.GetModuleDeclaration(i);
			if (!entry.ContainerId.HasValue)
			{
				return _modulesById[i]!;
			}
		}

		throw new InvalidOperationException("No global module found in metadata");
	}
}
