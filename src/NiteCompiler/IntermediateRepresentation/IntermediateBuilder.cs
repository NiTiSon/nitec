using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NiteCompiler.CodeAnalysis.Binding.BoundTree;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.Compilation;
using NiteCompiler.Metadata;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class IntermediateBuilder
{
	private readonly NiteCompilation _compilation;
	private readonly FunctionSymbol _function;
	private Dictionary<BoundStatement, SimpleBlock> _blocks = [];
	private int _nextValueId;

	private IntermediateBuilder(NiteCompilation compilation, FunctionSymbol function)
	{
		_compilation = compilation;
		_function = function;
	}

	public static byte[] Compile(NiteCompilation compilation, FunctionSymbol function, BoundBlock block,
		MetadataLibraryBuilder metadata)
	{
		IntermediateBuilder builder = new(compilation, function);

		using MemoryStream mem = new();
		using BinaryWriter writer = new(mem);

		Block entryBlock = builder.Build(block, metadata);
		foreach (Instruction instruction in entryBlock.Instructions)
		{
			instruction.Emit(writer);
		}

		return mem.ToArray();
	}

	public Block Build(BoundBlock block, MetadataLibraryBuilder metadata)
	{
		return null!;
	}

	private Value GetValue()
	{
		return new Value(_nextValueId++);
	}

	private void Link(SimpleBlock from, SimpleBlock to)
	{
		from.Successors.Add(to);
		to.Predecessors.Add(from);
	}
}