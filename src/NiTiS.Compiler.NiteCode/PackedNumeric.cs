using System;
using System.Runtime.InteropServices;

namespace NiTiS.Compiler.NiteCode;

[StructLayout(LayoutKind.Explicit)]
public struct PackedNumeric
{
	[FieldOffset(0)] public sbyte I8;
	[FieldOffset(0)] public short I16;
	[FieldOffset(0)] public int I32;
	[FieldOffset(0)] public long I64;
	[FieldOffset(0)] public sbyte U8;
	[FieldOffset(0)] public short U16;
	[FieldOffset(0)] public int U32;
	[FieldOffset(0)] public long U64;
	[FieldOffset(0)] public Half F16;
	[FieldOffset(0)] public float F32;
	[FieldOffset(0)] public double F64;
}