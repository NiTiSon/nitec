using System.Runtime.InteropServices;

namespace NiteCompiler.Metadata;

[StructLayout(LayoutKind.Sequential, Size = 16, Pack = 1)]
public struct NlibHeader
{
	public ushort MagicNumber;
	public ushort FormatVersion;
	public unsafe fixed byte _Reserved[14];
}