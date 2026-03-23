using System;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Binding.BoundTree;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.Compilation;
using NiteCompiler.Metadata;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class IntermediateBuilder
{
	private readonly NiteCompilation _compilation;
	private readonly FunctionSymbol _function;
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

		Block entryBlock = builder.Build(block, metadata);

		// Turn all blocks into byte sequence
		return [];
	}

	private Value GetValue()
	{
		return new Value(_nextValueId++);
	}

	public Block Build(BoundBlock block, MetadataLibraryBuilder metadata)
	{
		/*
		let a = 4;
		let b = 9;
		let c;
		if a < b {
			c = 760;
		} else {
			c = 69;
		}

		return c;
		*/

		// entry:
		//   $0 = load i32 4
		//   $1 = load i32 9
		//   $2 = cmp i32 $0 slt $1
		//   br i1 $2 if.then, if.else
		// if.then:
		//   $3 = load i32 760
		//   br if.after
		// if.else
		//   $3 = load i32 69
		//   br if.after
		// if.after
		//   $4 = phi [$3 if.then], [$4 if.else]
		//   ret $4
		return BuildBlock(block);
	}

	private Block BuildBlock(BoundBlock boundBlock)
	{
		SimpleBlock block = new();
		foreach (BoundStatement statement in boundBlock.Statements)
		{
			BuildStatement(block, statement);
		}

		return block;
	}

	private void BuildStatement(SimpleBlock block, BoundStatement statement)
	{
		switch (statement)
		{
			case BoundExpressionStatement expr:
				BuildExpression(block, expr.Expression);
				break;
			case BoundReturn @return:
				BuildReturn(block, @return);
				break;
			default:
				throw new UnreachableException();
		}
	}

	private Value BuildExpression(SimpleBlock block, BoundExpression expression)
	{
		switch (expression)
		{
			case BoundLiteral literal:
				return BuildLiteral(block, literal);
			case BoundBinaryExpression binary:
				return BuildBinaryExpression(block, binary);
			default:
				throw new UnreachableException();
		}
	}

	private void BuildReturn(SimpleBlock block, BoundReturn @return)
	{
		if (@return.Expression == null)
		{
			block.Instructions.Add(new RetInstruction(null));
			return;
		}

		Value retusa = BuildExpression(block, @return.Expression);
		block.Instructions.Add(new RetInstruction(retusa));
	}

	private Value BuildLiteral(SimpleBlock block, BoundLiteral literal)
	{
		Value imm = GetValue();
		LoadImmInstruction load = new(imm, literal.ConstantValue);
		block.Instructions.Add(load);
		return imm;
	}

	private Value BuildBinaryExpression(SimpleBlock block, BoundBinaryExpression binary)
	{
		Value lhs = BuildExpression(block, binary.Left);
		Value rhs = BuildExpression(block, binary.Right);
		Value result = GetValue();
		if (binary.Op.CorrespondingFunction == null) // builtin operator for builtin types
		{
			// For now only addition
			AddInstruction add = new(result, lhs, rhs);
			block.Instructions.Add(add);
			return result;
		}
		else
		{
			throw new NotImplementedException("Custom operators not implemented yet.");
		}
	}
}