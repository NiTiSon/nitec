using System.IO;

namespace NiteCompiler.IntermediateRepresentation.Nir;

internal abstract class Operand
{
	public abstract IValue Value { get; }
	public abstract bool IsMove { get; }
	public bool IsCopy => !IsMove;

	public abstract void Write(TextWriter writer);
}
