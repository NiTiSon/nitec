namespace NiteLang.Metadata;

public enum TableType : byte
{
	ConstantTable = 0,
	LibraryReference = 1,
	// LibraryDeclaration = 2,
	ModuleReference = 3,
	ModuleDeclaration = 4,
	TypeReference = 5,
	TypeDeclaration = 6,
	FieldReference = 7,
	FieldDeclaration = 8,
	FunctionReference = 9,
	FunctionDeclaration = 10,
	StringTable = 15,
}