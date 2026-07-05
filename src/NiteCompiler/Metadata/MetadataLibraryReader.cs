using System;
using System.Collections.Immutable;
using System.IO;

namespace NiteCompiler.Metadata;

internal sealed class MetadataLibraryReader
{
	public const int CurrentFormatVersion = 1;

	public int FormatVersion { get; }
	public string LibraryName { get; }

	private readonly string[] _strings;

	private readonly LibraryReferenceEntry[]? _libraryReferences;
	private readonly ModuleDeclarationEntry[]? _moduleDeclarations;
	private readonly ModuleReferenceEntry[]? _moduleReferences;
	private readonly TypeDeclarationEntry[]? _typeDeclarations;
	private readonly TypeDeclarationEntry[]? _typeReferences;
	private readonly FunctionDeclarationEntry[]? _functionDeclarations;
	private readonly FunctionReferenceEntry[]? _functionReferences;

	public MetadataLibraryReader(Stream stream)
	{
		using BinaryReader reader = new(stream);

		ReadHeader(reader, out int formatVersion, out uint libraryNameId);
		FormatVersion = formatVersion;
		_strings = ReadStringTable(reader);
		LibraryName = _strings[libraryNameId];

		while (reader.BaseStream.Position < reader.BaseStream.Length)
		{
			MetadataKind kind = (MetadataKind)reader.ReadByte();
			int entryCount = reader.Read7BitEncodedInt();

			switch (kind)
			{
				case MetadataKind.LibraryReference:
					_libraryReferences = ReadLibraryReferences(reader, entryCount);
					break;
				case MetadataKind.ModuleDeclaration:
					_moduleDeclarations = ReadModuleDeclarations(reader, entryCount);
					break;
				case MetadataKind.ModuleReference:
					_moduleReferences = ReadModuleReferences(reader, entryCount);
					break;
				case MetadataKind.TypeDeclaration:
					_typeDeclarations = ReadTypeDeclarations(reader, entryCount);
					break;
				case MetadataKind.TypeReference:
					_typeReferences = ReadTypeDeclarations(reader, entryCount);
					break;
				case MetadataKind.FunctionDeclaration:
					_functionDeclarations = ReadFunctionDeclarations(reader, entryCount);
					break;
				case MetadataKind.FunctionReference:
					_functionReferences = ReadFunctionReferences(reader, entryCount);
					break;
				default:
					throw new InvalidDataException($"Unknown metadata kind: {(byte)kind}");
			}
		}
	}

	public string GetString(uint id)
	{
		return _strings[id];
	}

	public int GetTableCount(MetadataKind kind)
	{
		return GetTable(kind)?.Length ?? 0;
	}

	public LibraryReferenceEntry GetLibraryReference(uint index)
	{
		return _libraryReferences?[index - 1] ?? throw new InvalidOperationException();
	}

	public ModuleDeclarationEntry GetModuleDeclaration(uint index)
	{
		return _moduleDeclarations?[index - 1] ?? throw new InvalidOperationException();
	}

	public ModuleReferenceEntry GetModuleReference(uint index)
	{
		return _moduleReferences?[index - 1] ?? throw new InvalidOperationException();
	}

	public TypeDeclarationEntry GetTypeDeclaration(uint index)
	{
		return _typeDeclarations?[index - 1] ?? throw new InvalidOperationException();
	}

	public TypeDeclarationEntry GetTypeReference(uint index)
	{
		return _typeReferences?[index - 1] ?? throw new InvalidOperationException();
	}

	public FunctionDeclarationEntry GetFunctionDeclaration(uint index)
	{
		return _functionDeclarations?[index - 1] ?? throw new InvalidOperationException();
	}

	public FunctionReferenceEntry GetFunctionReference(uint index)
	{
		return _functionReferences?[index - 1] ?? throw new InvalidOperationException();
	}

	private static void ReadHeader(BinaryReader reader, out int formatVersion, out uint libraryNameId)
	{
		Span<byte> magic = stackalloc byte[4];
		reader.Read(magic);
		if (magic[0] != 'n' || magic[1] != 'l' || magic[2] != 'i' || magic[3] != 'b')
		{
			throw new InvalidDataException("Invalid .nlib magic number");
		}

		formatVersion = reader.ReadUInt16();
		if (formatVersion != CurrentFormatVersion)
		{
			throw new NotSupportedException($".nlib format version {formatVersion} is not supported");
		}

		libraryNameId = reader.ReadUInt32();
		reader.ReadBytes(22);
	}

	private static string[] ReadStringTable(BinaryReader reader)
	{
		int count = reader.Read7BitEncodedInt();
		string[] strings = new string[count + 1];

		for (int i = 1; i <= count; i++)
		{
			strings[i] = reader.ReadString();
		}

		return strings;
	}

	private static LibraryReferenceEntry[] ReadLibraryReferences(BinaryReader reader, int count)
	{
		LibraryReferenceEntry[] entries = new LibraryReferenceEntry[count];
		for (int i = 0; i < count; i++)
		{
			entries[i] = new LibraryReferenceEntry(reader.ReadUInt32());
		}

		return entries;
	}

	private static ModuleDeclarationEntry[] ReadModuleDeclarations(BinaryReader reader, int count)
	{
		ModuleDeclarationEntry[] entries = new ModuleDeclarationEntry[count];
		for (int i = 0; i < count; i++)
		{
			uint rawContainerId = reader.ReadUInt32();
			uint nameId = reader.ReadUInt32();
			MetadataId? containerId = rawContainerId != 0 ? new MetadataId(rawContainerId) : null;
			entries[i] = new ModuleDeclarationEntry(containerId, nameId);
		}

		return entries;
	}

	private static ModuleReferenceEntry[] ReadModuleReferences(BinaryReader reader, int count)
	{
		ModuleReferenceEntry[] entries = new ModuleReferenceEntry[count];
		for (int i = 0; i < count; i++)
		{
			MetadataId containerId = reader.ReadUInt32();
			MetadataId libraryId = reader.ReadUInt32();
			uint nameId = reader.ReadUInt32();
			entries[i] = new ModuleReferenceEntry(containerId, libraryId, nameId);
		}

		return entries;
	}

	private static TypeDeclarationEntry[] ReadTypeDeclarations(BinaryReader reader, int count)
	{
		TypeDeclarationEntry[] entries = new TypeDeclarationEntry[count];
		for (int i = 0; i < count; i++)
		{
			MetadataId containerId = reader.ReadUInt32();
			uint nameId = reader.ReadUInt32();
			TypeMetadataFlags flags = (TypeMetadataFlags)reader.ReadUInt16();
			SpecialType specialType = SpecialType.None;

			if (flags.HasFlag(TypeMetadataFlags.IsSpecialType))
			{
				specialType = (SpecialType)reader.ReadByte();
			}

			entries[i] = new TypeDeclarationEntry(containerId, nameId, flags, specialType);
		}

		return entries;
	}

	private static FunctionDeclarationEntry[] ReadFunctionDeclarations(BinaryReader reader, int count)
	{
		FunctionDeclarationEntry[] entries = new FunctionDeclarationEntry[count];
		for (int i = 0; i < count; i++)
		{
			MetadataId containerId = reader.ReadUInt32();
			uint nameId = reader.ReadUInt32();
			byte bodyPresent = reader.ReadByte();
			ImmutableArray<byte>? body = null;

			if (bodyPresent != 0)
			{
				int bodyLength = reader.Read7BitEncodedInt();
				byte[] bodyBytes = new byte[bodyLength];
				reader.Read(bodyBytes);
				body = ImmutableArray.Create(bodyBytes);
			}

			entries[i] = new FunctionDeclarationEntry(containerId, nameId, body);
		}

		return entries;
	}

	private static FunctionReferenceEntry[] ReadFunctionReferences(BinaryReader reader, int count)
	{
		FunctionReferenceEntry[] entries = new FunctionReferenceEntry[count];
		for (int i = 0; i < count; i++)
		{
			entries[i] = new FunctionReferenceEntry(reader.ReadUInt32(), reader.ReadUInt32());
		}

		return entries;
	}

	private Array? GetTable(MetadataKind kind)
	{
		return kind switch
		{
			MetadataKind.LibraryReference => _libraryReferences,
			MetadataKind.ModuleDeclaration => _moduleDeclarations,
			MetadataKind.ModuleReference => _moduleReferences,
			MetadataKind.TypeDeclaration => _typeDeclarations,
			MetadataKind.TypeReference => _typeReferences,
			MetadataKind.FunctionDeclaration => _functionDeclarations,
			MetadataKind.FunctionReference => _functionReferences,
			_ => null
		};
	}
}

internal readonly struct LibraryReferenceEntry(uint nameId)
{
	public uint NameId { get; } = nameId;
}

internal readonly struct ModuleDeclarationEntry(MetadataId? containerId, uint nameId)
{
	public MetadataId? ContainerId { get; } = containerId;
	public uint NameId { get; } = nameId;
}

internal readonly struct ModuleReferenceEntry(MetadataId containerId, MetadataId libraryId, uint nameId)
{
	public MetadataId ContainerId { get; } = containerId;
	public MetadataId LibraryId { get; } = libraryId;
	public uint NameId { get; } = nameId;
}

internal readonly struct TypeDeclarationEntry(MetadataId containerId, uint nameId, TypeMetadataFlags flags, SpecialType specialType)
{
	public MetadataId ContainerId { get; } = containerId;
	public uint NameId { get; } = nameId;
	public TypeMetadataFlags Flags { get; } = flags;
	public SpecialType SpecialType { get; } = specialType;
}

internal readonly struct FunctionDeclarationEntry(MetadataId containerId, uint nameId, ImmutableArray<byte>? body)
{
	public MetadataId ContainerId { get; } = containerId;
	public uint NameId { get; } = nameId;
	public ImmutableArray<byte>? Body { get; } = body;
}

internal readonly struct FunctionReferenceEntry(MetadataId containerId, uint nameId)
{
	public MetadataId ContainerId { get; } = containerId;
	public uint NameId { get; } = nameId;
}
