using System;

namespace NiTiS.Language.Runtime;

[Flags]
public enum RuntimeFeatures : ulong
{
	None = 0,

	Generics = 1,
	StaticConstructors = 1 << 1,
	ThreadLocals = 1 << 2,
}