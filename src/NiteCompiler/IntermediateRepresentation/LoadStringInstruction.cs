using System.Diagnostics;
using System.IO;
using System.Text;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class LoadStringInstruction(IValue output, string stringData, StringLiteralType encoding) : Instruction
{
	public override bool IsBranch => false;

	public readonly IValue Output = output;
	public readonly string StringData = stringData;
	public readonly StringLiteralType Encoding = encoding;

	public override void Emit(BinaryWriter writer)
	{
		byte[] bytes = Encoding switch
		{
			StringLiteralType.Unicode16 => System.Text.Encoding.Unicode.GetBytes(StringData),
			StringLiteralType.Unicode32 => System.Text.Encoding.UTF32.GetBytes(StringData),
			_ => System.Text.Encoding.UTF8.GetBytes(StringData)
		};

		Bytecode opcode = Encoding switch
		{
			StringLiteralType.Unicode16 => Bytecode.LoadStr16,
			StringLiteralType.Unicode32 => Bytecode.LoadStr32,
			_ => Bytecode.LoadStr8
		};

		writer.Write((byte)opcode);
		writer.Write7BitEncodedInt(bytes.Length);
		writer.Write(bytes);
	}

	public override void Write(TextWriter writer)
	{
		Output.Write(writer);
		string encodingName = Encoding switch
		{
			StringLiteralType.Unicode16 => "u16",
			StringLiteralType.Unicode32 => "u32",
			_ => "u8"
		};
		writer.Write($" = load string {encodingName} \"{StringData}\"");
	}
}
