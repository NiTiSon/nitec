using System.IO;
using NiteCompiler.CodeAnalysis;

namespace NiteCompiler.IntermediateRepresentation;

internal class LoadImmInstruction(Value output, ConstantValue constant) : Instruction
{
	public readonly Value Output = output;
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
}