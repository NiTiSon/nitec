namespace NiteCompiler.CodeAnalysis;

internal static class WellKnownSemantic
{
	public const string GlobalModuleName = "<global>";
	public const string NumericsModuleName = "std::numerics";
	public const string StandardModuleName = "std";

	public const string I8TypeName = "SInt8";
	public const string I16TypeName = "SInt16";
	public const string I32TypeName = "SInt32";
	public const string I64TypeName = "SInt64";
	public const string U8TypeName = "UInt8";
	public const string U16TypeName = "UInt16";
	public const string U32TypeName = "UInt32";
	public const string U64TypeName = "UInt64";
	public const string F16TypeName = "Float16";
	public const string F32TypeName = "Float32";
	public const string F64TypeName = "Float64";
	public const string NeverReturnTypeName = "NeverReturn";
	public const string ReferenceTypeName = "Reference<3>";
	public const string BoxReferenceTypeName = "BoxReference<3>";
	public const string PointerTypeName = "Pointer<3>";
}