using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NiteCompiler.CodeAnalysis.Binding;

namespace NiteCompiler.IntermediateRepresentation.ControlFlow;

[DebuggerDisplay("{DebuggerDisplay(),nq}")]
internal sealed class BasicBlock(string name)
{
	public string? Name { get; } = name;
	public List<BoundStatement> Statements { get; } = [];
	public ControlFlowTerminator? Terminator { get; internal set; }

	public List<BasicBlock> Predecessors { get; } = [];
	public List<BasicBlock> Successors { get; } = [];

	private string DebuggerDisplay()
	{
		StringWriter sw = new();

		sw.Write("Block ");
		sw.Write(Name);

		if (Predecessors.Count > 0)
		{
			sw.Write(" in(");
			sw.Write(string.Join(", ", Predecessors.Select(t => t.Name)));
			sw.Write(")");
		}

		if (Successors.Count > 0)
		{
			sw.Write(" out(");
			sw.Write(string.Join(", ", Successors.Select(t => t.Name)));
			sw.Write(")");
		}

		return sw.ToString();
	}
}