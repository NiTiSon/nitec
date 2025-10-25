namespace NiteLang.Metadata;

public enum TableType : byte
{
	Constant = 0,
	LibraryReference = 1,
	// LibraryDeclaration = 2, // Library declaration is embedded in header; Maybe allow to contain multiple libraries within one file
	ModuleReference = 3,
	ModuleDeclaration = 4,
	TypeReference = 5,
	TypeDeclaration = 6,
	FieldReference = 7,
	FieldDeclaration = 8,
	FunctionReference = 9,
	FunctionDeclaration = 10,
	PropertyReference = 11,
	PropertyDeclaration = 12,
	StringTable = 15,
}