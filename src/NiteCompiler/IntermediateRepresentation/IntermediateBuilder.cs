using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.Compilation;
using NiteCompiler.Metadata;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class IntermediateBuilder
{
	private readonly NiteCompilation _compilation;
	private readonly FunctionSymbol _function;

	private IntermediateBuilder(NiteCompilation compilation, FunctionSymbol function)
	{
		_compilation = compilation;
		_function = function;
	}

	public static byte[] Compile(NiteCompilation compilation, FunctionSymbol function, BoundBlock block,
		MetadataLibraryBuilder metadata)
	{

		return [];
		// IntermediateBuilder builder = new(compilation, function);
		//
		// using MemoryStream mem = new();
		// using BinaryWriter writer = new(mem);
		//
		// Block entryBlock = builder.Build(block, metadata);
		// foreach (Instruction instruction in entryBlock.Instructions)
		// {
		// 	instruction.Emit(writer);
		// }
		//
		// return mem.ToArray();
	}
}