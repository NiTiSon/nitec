using Microsoft.Extensions.Primitives;

namespace Nlr.Compiler.CodeAnalysis.NiteCode;

internal struct TokenInfo
{
	public SyntaxKind Kind;
	public StringSegment Text;
	public string StringValue; // nlr strings are utf8, not utf16 (conversion later, on compilation)
	public byte CharValue; // nlr characters are utf8, not utf16

	public byte U8Value;
	public ushort U16Value;
	public uint U32Value;
	public ulong U64Value;

	public sbyte I8Value;
	public short I16Value;
	public int I32Value;
	public long I64Value;

	public float F32Value;
	public double F64Value;
}