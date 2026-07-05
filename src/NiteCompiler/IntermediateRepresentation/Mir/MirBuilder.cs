using System;
using System.Collections.Generic;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Binding.Operators;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.Compilation;
using NiteCompiler.IntermediateRepresentation.ControlFlow;

namespace NiteCompiler.IntermediateRepresentation.Mir;

internal sealed class MirBuilder
{
	private readonly NiteCompilation _compilation;
	private readonly FunctionSymbol _function;
	private readonly ControlFlowGraph _cfg;
	private Dictionary<LocalVariableOrParameterSymbol, TempValue> _addrTable = [];

	private MirBuilder(NiteCompilation compilation, FunctionSymbol function, ControlFlowGraph cfg)
	{
		_compilation = compilation;
		_function = function;
		_cfg = cfg;
	}

	public static FunctionMir Build(NiteCompilation compilation, FunctionSymbol function, ControlFlowGraph cfg)
	{
		var builder = new MirBuilder(compilation, function, cfg);
		return builder.CreateMir();
	}

	private FunctionMir CreateMir()
	{
		FunctionMir mir = new(_function);
		foreach (BasicBlock bb in _cfg.Blocks)
		{
			mir.Blocks[bb] = new MirBlock(bb);
		}

		CreateStackAlloc(mir.Blocks[_cfg.Entry]);
		mir.AddressTable = _addrTable;

		foreach (BasicBlock bb in _cfg.Blocks)
		{
			Rename(bb, mir);
		}

		return mir;
	}

	private int _tempId;
	private readonly HashSet<string> _tempNames = [];
	private TempValue GetNewTemp(TypeSymbol type, string? name = null)
	{
		if (name != null)
		{
			if (!_tempNames.Add(name))
			{
				name += _tempId;
			}
		}

		return new TempValue(_tempId++, type, name);
	}

	private void CreateStackAlloc(MirBlock mir)
	{
		foreach (ParameterSymbol parameterSymbol in _function.Parameters)
		{
			TypeSymbol ptr = _compilation.CreatePointerType(parameterSymbol.Type, true, false);
			TempValue temp = GetNewTemp(ptr);
			mir.Instructions.Add(new StackAllocInstruction(temp, parameterSymbol.Type));
			_addrTable[parameterSymbol] = temp;
		}

		var locals = CollectAllVariablesAndParameters();
		foreach (LocalVariableSymbol local in locals)
		{
			TypeSymbol ptr = _compilation.CreatePointerType(local.Type, true, false);
			TempValue temp = GetNewTemp(ptr);
			mir.Instructions.Add(new StackAllocInstruction(temp, local.Type));
			_addrTable[local] = temp;
		}

		foreach (ParameterSymbol parameterSymbol in _function.Parameters)
		{
			TempValue temp = GetNewTemp(parameterSymbol.Type);
			mir.Instructions.Add(new LoadParamInstruction(temp, parameterSymbol));
			mir.Instructions.Add(new StoreInstruction(temp, _addrTable[parameterSymbol]));
		}
	}

	private IEnumerable<LocalVariableSymbol> CollectAllVariablesAndParameters()
	{
		HashSet<LocalVariableSymbol> set = [];

		foreach (var block in _cfg.Blocks)
		{
			foreach (var stmt in block.Statements)
			{
				if (stmt is BoundLocalVariableDeclarationStatement var)
				{
					set.Add(var.Local);
				}
			}
		}

		return set;
	}

	private void Rename(BasicBlock bbEntry, FunctionMir mir)
	{
		var mirBlock = mir.Blocks[bbEntry];

		foreach (BoundStatement statement in bbEntry.Statements)
		{
			RewriteStatement(statement, mirBlock);
		}
		Debug.Assert(bbEntry.Terminator is not null);
		{
			RewriteTerminator(bbEntry.Terminator, mirBlock);
		}
	}

	private void RewriteStatement(BoundStatement statement, MirBlock mirBlock)
	{
		switch (statement)
		{
			case BoundExpressionStatement exprStmt:
				RewriteExpression(exprStmt.Expression, mirBlock);
				break;
			case BoundLocalVariableDeclarationStatement declStmt:
				if (declStmt.Initializer != null)
				{
					TempValue value = RewriteExpression(declStmt.Initializer, mirBlock);
					TempValue addr = _addrTable[declStmt.Local];
					mirBlock.Instructions.Add(new StoreInstruction(value, addr));
				}
				break;
			case BoundEmptyStatement:
				break;
			default:
				throw new UnreachableException($"RewriteStatement({statement.GetType()})");
		}
	}

	private void RewriteTerminator(ControlFlowTerminator terminator, MirBlock block)
	{
		switch (terminator)
		{
			case ConditionalBranchTerminator condBr:
				TempValue cond = RewriteExpression(condBr.Condition, block);
				block.Instructions.Add(new CondBrInstruction(cond, condBr.Then, condBr.Else));
				break;
			case ReturnTerminator ret:
				if (ret.Expression != null)
				{
					TempValue retValue = RewriteExpression(ret.Expression, block);
					block.Instructions.Add(new RetInstruction(retValue));
				}
				else
				{
					block.Instructions.Add(new RetInstruction(null));
				}
				break;
			case BranchTerminator br:
				block.Instructions.Add(new BrInstruction(br.Target));
				break;
			default: throw new UnreachableException($"RewriteTerminator({terminator.GetType()})");
		}
	}

	private TempValue RewriteExpression(BoundExpression expression, MirBlock block)
	{
		return expression switch
		{
			BoundLiteral literal => EmitLiteral(literal, block),
			BoundAddressOfExpression addressOf => EmitAddressOfExpression(addressOf, block),
			BoundDereferenceExpression dereference => EmitDereferenceExpression(dereference, block),
			BoundUnaryExpression unary => EmitUnaryExpression(unary, block),
			BoundBinaryExpression binary => EmitBinaryExpression(binary, block),
			BoundAssignment assignment => EmitAssignmentExpression(assignment, block),
			BoundCompoundAssignment compoundAssignment => throw new InvalidOperationException("Unlowered BoundTree is passed to the MirBuilder."),
			BoundCall call => EmitCall(call, block),
			BoundParameter parameter => EmitParameter(parameter, block),
			BoundLocal local => EmitLocal(local, block),

			_ => throw new UnreachableException($"RewriteExpression({expression.GetType()})")
		};
	}

	private TempValue EmitAddressOfExpression(BoundAddressOfExpression addressOf, MirBlock block)
	{
		TempValue output = GetNewTemp(addressOf.Type);
		LocalVariableOrParameterSymbol symbol = addressOf.Expression switch
		{
			BoundLocal local => local.Local,
			BoundParameter parameter => parameter.Parameter,
			_ => throw new UnreachableException($"EmitAddressOfExpression({addressOf.Expression.GetType()})")
		};
		block.Instructions.Add(new AddressOfInstruction(output, symbol));
		return output;
	}

	private TempValue EmitDereferenceExpression(BoundDereferenceExpression dereference, MirBlock block)
	{
		TempValue addr = RewriteExpression(dereference.Expression, block);
		TempValue load = GetNewTemp(dereference.Type);
		block.Instructions.Add(new LoadInstruction(load, addr));
		return load;
	}

	private TempValue EmitLiteral(BoundLiteral literal, MirBlock block)
	{
		switch (literal.Type.SpecialType)
		{
			case (>= SpecialType.StdNumericsSInt8 and <= SpecialType.StdNumericsFloat64)
				or SpecialType.StdBoolean:
			{
				TempValue output = GetNewTemp(literal.Type);
				LoadImmInstruction imm = new(output, literal.ConstantValue);
				block.Instructions.Add(imm);
				return output;
			}
			case not SpecialType.None:
				throw new ArgumentException($"Imposible literal type: {literal.Type.SpecialType}");
			default:
				throw new UnreachableException($"EmitLiteral({literal.Type.ToDisplayString()})");
		}
	}

	private TempValue EmitUnaryExpression(BoundUnaryExpression unary, MirBlock block)
	{
		if (unary.Op.CorrespondingFunction is not null) // user-defined operator
		{
			throw new NotImplementedException("call instruction is not implemented yet");
		}

		TempValue value = RewriteExpression(unary.Expression, block);

		if (unary.Op.Kind == UnaryOperatorKind.Plus)
		{
			// +expr is a nope operation, no need to allocate new value
			return value;
		}

		TempValue result = GetNewTemp(unary.Type);
		UnaryInstruction instruction = unary.Op.Kind switch
		{
			UnaryOperatorKind.Negate => new NegInstruction(result, value),
			UnaryOperatorKind.BitwiseNot or UnaryOperatorKind.LogicalNot => new NotInstruction(result, value),
			_ => throw new UnreachableException($"EmitUnaryExpression({unary.Op.Kind})")
		};
		block.Instructions.Add(instruction);
		return result;
	}

	private TempValue EmitBinaryExpression(BoundBinaryExpression binary, MirBlock block)
	{
		if (binary.Op.CorrespondingFunction is not null) // user-defined operator
		{
			throw new NotImplementedException("call instruction is not implemented yet");
		}

		TempValue lhs = RewriteExpression(binary.Left, block);
		TempValue rhs = RewriteExpression(binary.Right, block);
		TempValue result = GetNewTemp(binary.Type);

		BinaryInstruction instruction = binary.Op.Kind switch
		{
			BinaryOperatorKind.Addition => new AddInstruction(result, lhs, rhs),
			BinaryOperatorKind.Subtraction => new SubInstruction(result, lhs, rhs),
			BinaryOperatorKind.Multiplication => new MulInstruction(result, lhs, rhs),
			BinaryOperatorKind.Division => new DivInstruction(result, lhs, rhs),
			BinaryOperatorKind.Modulo => new ModInstruction(result, lhs, rhs),
			BinaryOperatorKind.LeftArithmeticShift => new SalInstruction(result, lhs, rhs),
			BinaryOperatorKind.RightArithmeticShift => new SarInstruction(result, lhs, rhs),
			BinaryOperatorKind.RightUnsignedShift => new ShrInstruction(result, lhs, rhs),
			BinaryOperatorKind.Equal => new CmpEqInstruction(result, lhs, rhs),
			BinaryOperatorKind.NotEqual => new CmpNeqInstruction(result, lhs, rhs),
			BinaryOperatorKind.Greater => new CmpGtInstruction(result, lhs, rhs),
			BinaryOperatorKind.Less => new CmpLtInstruction(result, lhs, rhs),
			BinaryOperatorKind.GreaterOrEqual => new CmpGeInstruction(result, lhs, rhs),
			BinaryOperatorKind.LessOrEqual => new CmpLeInstruction(result, lhs, rhs),
			BinaryOperatorKind.And => new AndInstruction(result, lhs, rhs),
			BinaryOperatorKind.Xor => new XorInstruction(result, lhs, rhs),
			BinaryOperatorKind.Or => new OrInstruction(result, lhs, rhs),
			// tilde is never a builtin operator: no underlying instruction, only an invokable function
			_ => throw new UnreachableException($"EmitBinaryExpression({binary.Op.Kind})")
		};

		block.Instructions.Add(instruction);
		return result;
	}

	private TempValue EmitAssignmentExpression(BoundAssignment assignment, MirBlock block)
	{
		TempValue value = RewriteExpression(assignment.Right, block);
		TempValue addr = GetAssignmentTargetAddress(assignment.Left, block);
		block.Instructions.Add(new StoreInstruction(value, addr));
		return value;
	}

	private TempValue GetAssignmentTargetAddress(BoundExpression expression, MirBlock block)
	{
		return expression switch
		{
			BoundLocal local => _addrTable[local.Local],
			BoundParameter parameter => _addrTable[parameter.Parameter],
			BoundDereferenceExpression dereference => RewriteExpression(dereference.Expression, block),
			_ => throw new UnreachableException($"GetAssignmentTargetAddress({expression.GetType()})")
		};
	}

	private TempValue EmitCall(BoundCall call, MirBlock block)
	{
		TempValue ret = GetNewTemp(call.Function.ReturnType ?? _compilation.GetSpecialType(SpecialType.StdNeverReturn));
		ArrayBuilder<TempValue> arguments = ArrayBuilder<TempValue>.GetInstance();
		foreach (var arg in call.Arguments)
		{
			arguments.Add(RewriteExpression(arg, block));
		}

		block.Instructions.Add(new CallInstruction(ret, call.Function, arguments.ToImmutableAndFree()));

		return ret;
	}

	private TempValue EmitParameter(BoundParameter parameter, MirBlock block)
	{
		TempValue addr = _addrTable[parameter.Parameter];
		TempValue load = GetNewTemp(parameter.Parameter.Type);

		block.Instructions.Add(new LoadInstruction(load, addr));

		return load;
	}

	private TempValue EmitLocal(BoundLocal local, MirBlock block)
	{
		TempValue addr = _addrTable[local.Local];
		TempValue load = GetNewTemp(local.Local.Type);

		block.Instructions.Add(new LoadInstruction(load, addr));

		return load;
	}

}