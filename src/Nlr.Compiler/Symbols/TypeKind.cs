namespace Nlr.Compiler.NiteCode.Symbols;

public enum TypeKind
{
	Unknown,
	Type,
	ReferenceType,
	PointerType,
	SliceType,
	ArrayType,
	InterfaceType,
	EnumType,
}