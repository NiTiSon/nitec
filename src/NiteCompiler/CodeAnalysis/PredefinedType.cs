using System;

namespace NiteCompiler.CodeAnalysis;

public enum PredefinedType
{
	I8,
	I16,
	I32,
	I64,
	[Obsolete("Please, do not use this.")]
	NotUsed01,
	U8,
	U16,
	U32,
	U64,
	[Obsolete("Please, do not use this.")]
	NotUsed02,
	F16,
	F32,
	F64,
	Void,
	Never
}