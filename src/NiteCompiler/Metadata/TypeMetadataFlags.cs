using System;

namespace NiteCompiler.Metadata;

[Flags]
internal enum TypeMetadataFlags : ushort
{
	Abstract = 0x01,
	Sealed = 0x02,
	Static = Abstract | Sealed,
	IsSpecialType = 0x04,
}