namespace NiteCode.Compiler.CodeAnalysis.Symbols;

public enum Accessibility
{
	// ..VV - defines accessibility inside package
	// VV.. - defines accessibility outside package
	// 00 - visible for members inside same conteiner
	// 01 - visible for everyone
	// 10 - visible for children

	Public = 0b0101,
	Protected = 0b1010,
	Private = 0b0000,
	Homie = 0b1001,
	Family = 0b0010,
}