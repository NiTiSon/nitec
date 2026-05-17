using System.Diagnostics;
using System.IO;
using NiteCompiler.CodeAnalysis;

namespace NiteCompiler.IntermediateRepresentation;

internal class LoadImmInstruction(IValue output, ConstantValue constant) : Instruction
{
	public override bool IsBranch => false;

	public readonly IValue Output = output;
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
		writer.Write(" = load imm ");
		SpecialType specialType = Output.Type.SpecialType;
		if (specialType == SpecialType.StdBoolean)
		{
			writer.Write(Constant.Bool ? "true" : "false");
		}
		else
		{
			if (specialType.IsSignedIntegral)
			{
				writer.Write(Constant.S64);
			}
			else
			{
				Debug.Assert(specialType.IsUnsignedIntegral);
				writer.Write(Constant.U64);
			}
		}
	}
}