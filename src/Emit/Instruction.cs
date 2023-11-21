using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NiteCompiler.Emit.FlowControl;

namespace NiteCompiler.Emit;

public readonly struct Instruction
{
	public static readonly Instruction
		// Opcode pack №0
		Nope,
		LoadThis,
		LoadArg,
		LoadLocal,
		Return,
		Call,
		CallVirt,
		PushI32,
		PushI64,
		Conv8,
		Conv16,
		Conv32,
		Conv64,
		ConvS,
		ConvU,
		// ...
		;

	public readonly string Name;
	public readonly byte Code;
	public readonly FlowControl FlowControl;

	private Instruction(string name, byte code, FlowControl flowControl)
	{
		Name = name;
		Code = code;
		FlowControl = flowControl;
	}

	static Instruction()
	{
		Nope		= new("nope",	0x00, Next);
		LoadThis	= new("load.this",0x01, Next);
		LoadArg		= new("load.arg",	0x02, Next);
		LoadLocal	= new("load.loc",	0x03, Next);
		Return		= new("ret",		0x04, FlowControl.Return);
		Call		= new("call",		0x05, FlowControl.Call);
		CallVirt	= new("call.virt",	0x06, FlowControl.Call);
		PushI32		= new("push.int32",	0x07, Next);
		PushI64		= new("push.int64",	0x08, Next);
		Conv8		= new("conv.8",	0x09, Next);
		Conv32		= new("conv.16",	0x0A, Next);
		Conv64		= new("conv.32",	0x0B, Next);
		Conv32		= new("conv.64",	0x0C, Next);
		ConvU		= new("conv.sign",	0x0C, Next);
		ConvS		= new("conv.unsign",	0x0C, Next);
	}
}