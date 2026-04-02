using System.IO;
using NiteCompiler.CodeAnalysis;
using NiteCompiler.IntermediateRepresentation.Ssa;

namespace NiteCompiler.IntermediateRepresentation;

internal class LoadImmInstruction(SsaTemp output, ConstantValue constant) : Instruction
{
	public override bool IsBranch => false;

	public readonly SsaTemp Output = output;
	public readonly ConstantValue Constant = constant;

	public override void Emit(BinaryWriter writer)
	{
		switch (Constant.SpecialType)
		{
			case SpecialType.StdNumericsSInt32:
				writer.Write((byte)Bytecode.LoadS32);
				writer.Write7BitEncodedInt(Constant.S32);
				break;
		}
	}

	public override void Write(TextWriter writer)
	{
		Output.Write(writer);
		writer.Write(" = load ");
		writer.Write(Constant.U32);
	}
}