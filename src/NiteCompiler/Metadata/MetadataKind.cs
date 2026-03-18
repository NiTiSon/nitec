using System;

namespace NiteCompiler.Metadata;

/// <summary>
/// The unique number that indicates the metadata entity.
/// </summary>
internal enum MetadataKind : byte
{
	// Library declaration is always a singleton and stored in library header, so it's never appeared in tables
	// LibraryDeclaration = 0,
	LibraryReference = 1,
	ModuleDeclaration = 2,
	ModuleReference = 3,
	TypeDeclaration = 4,
	TypeReference = 5,
	FunctionDeclaration = 6,
	FunctionReference = 7,
}

internal static class MetadataKindExtensions
{
	extension(MetadataKind kind)
	{
		public bool IsReference => (int)kind % 2 != 0;
		public bool IsDeclaration => (int)kind % 2 == 0;

		public static int Count => Enum.GetValues<MetadataKind>().Length;
	}
}