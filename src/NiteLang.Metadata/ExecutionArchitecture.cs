namespace NiteLang.Metadata;

public enum ExecutionArchitecture : byte
{
	None = 0,
	/// <summary>
	/// NiTiS bytecode.
	/// </summary>
	Nbc = 0x01,
	X86 = 0x10,
	Amd64 = 0x11,
	X86_64 = Amd64,
	Arm = 0x12,
	Arm64 = 0x13,
}