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
	private HashSet<LocalVariableOrParameterSymbol> _ssaVariables = [];
	private readonly HashSet<LocalVariableOrParameterSymbol> _addressTaken = [];
	private Dictionary<LocalVariableOrParameterSymbol, Stack<TempValue>> _versionStack = [];
	private Dictionary<BasicBlock, Dictionary<LocalVariableOrParameterSymbol, TempValue>> _phiNodes = [];
	private Dictionary<LocalVariableOrParameterSymbol, TempValue> _initialVersions = [];

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

		AnalyzeAddressTaken();
		ClassifyVariables();

		CreatePrologue(mir.Blocks[_cfg.Entry]);
		mir.AddressTable = _addrTable;

		if (_ssaVariables.Count > 0)
		{
			InsertPhiNodes(mir);
		}

		RenameAll(mir);

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

	private void AnalyzeAddressTaken()
	{
		foreach (BasicBlock block in _cfg.Blocks)
		{
			foreach (BoundStatement stmt in block.Statements)
			{
				if (stmt is BoundExpressionStatement exprStmt)
				{
					FindAddressTakenInExpression(exprStmt.Expression);
				}
				else if (stmt is BoundLocalVariableDeclarationStatement declStmt && declStmt.Initializer != null)
				{
					FindAddressTakenInExpression(declStmt.Initializer);
				}
			}

			if (block.Terminator is ReturnTerminator ret && ret.Expression != null)
			{
				FindAddressTakenInExpression(ret.Expression);
			}
			else if (block.Terminator is ConditionalBranchTerminator condBr)
			{
				FindAddressTakenInExpression(condBr.Condition);
			}
		}
	}

	private void FindAddressTakenInExpression(BoundExpression expr)
	{
		if (expr is BoundAddressOfExpression addrOf)
		{
			LocalVariableOrParameterSymbol? variable = addrOf.Expression switch
			{
				BoundMove move => move.Variable,
				BoundCopy copy => copy.Variable,
				_ => null
			};
			if (variable != null)
			{
				_addressTaken.Add(variable);
			}
			return;
		}

		if (expr is BoundAssignment assignment)
		{
			FindAddressTakenInExpression(assignment.Left);
			FindAddressTakenInExpression(assignment.Right);
		}
		else if (expr is BoundBinaryExpression binary)
		{
			FindAddressTakenInExpression(binary.Left);
			FindAddressTakenInExpression(binary.Right);
		}
		else if (expr is BoundUnaryExpression unary)
		{
			FindAddressTakenInExpression(unary.Expression);
		}
		else if (expr is BoundCall call)
		{
			foreach (var arg in call.Arguments)
			{
				FindAddressTakenInExpression(arg);
			}
		}
		else if (expr is BoundDereferenceExpression deref)
		{
			FindAddressTakenInExpression(deref.Expression);
		}
	}

	private void ClassifyVariables()
	{
		foreach (ParameterSymbol param in _function.Parameters)
		{
			if (!_addressTaken.Contains(param))
			{
				_ssaVariables.Add(param);
			}
		}

		foreach (LocalVariableSymbol local in CollectAllVariablesAndParameters())
		{
			if (!_addressTaken.Contains(local))
			{
				_ssaVariables.Add(local);
			}
		}
	}

	private void CreatePrologue(MirBlock entryMir)
	{
		foreach (ParameterSymbol param in _function.Parameters)
		{
			if (_ssaVariables.Contains(param))
			{
				TempValue paramValue = GetNewTemp(param.Type, param.Name);
				entryMir.Instructions.Add(new LoadParamInstruction(paramValue, param));
				_initialVersions[param] = paramValue;
			}
			else
			{
				TypeSymbol ptr = _compilation.CreatePointerType(param.Type, true, false);
				TempValue temp = GetNewTemp(ptr);
				entryMir.Instructions.Add(new StackAllocInstruction(temp, param.Type));
				_addrTable[param] = temp;
			}
		}

		foreach (LocalVariableSymbol local in CollectAllVariablesAndParameters())
		{
			if (!_ssaVariables.Contains(local))
			{
				TypeSymbol ptr = _compilation.CreatePointerType(local.Type, true, false);
				TempValue temp = GetNewTemp(ptr);
				entryMir.Instructions.Add(new StackAllocInstruction(temp, local.Type));
				_addrTable[local] = temp;
			}
		}

		foreach (ParameterSymbol param in _function.Parameters)
		{
			if (!_ssaVariables.Contains(param))
			{
				TempValue temp = GetNewTemp(param.Type);
				entryMir.Instructions.Add(new LoadParamInstruction(temp, param));
				entryMir.Instructions.Add(new StoreInstruction(temp, _addrTable[param]));
			}
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

	private void InsertPhiNodes(FunctionMir mir)
	{
		foreach (var variable in _ssaVariables)
		{
			InsertPhiForVariable(variable, mir);
		}
	}

	private void InsertPhiForVariable(LocalVariableOrParameterSymbol variable, FunctionMir mir)
	{
		HashSet<BasicBlock> defBlocks = FindDefBlocks(variable);

		if (defBlocks.Count == 0)
			return;

		var hasPhi = new HashSet<BasicBlock>();
		var workList = new Queue<BasicBlock>(defBlocks);

		while (workList.Count > 0)
		{
			BasicBlock block = workList.Dequeue();

			if (!_cfg.DominanceFrontier.TryGetValue(block, out var frontier))
				continue;

			foreach (BasicBlock frontierBlock in frontier)
			{
				if (!hasPhi.Add(frontierBlock))
					continue;

				AddPhiForVariable(variable, frontierBlock, mir);

				if (!defBlocks.Contains(frontierBlock))
					workList.Enqueue(frontierBlock);
			}
		}
	}

	private HashSet<BasicBlock> FindDefBlocks(LocalVariableOrParameterSymbol variable)
	{
		var defBlocks = new HashSet<BasicBlock>();

		foreach (BasicBlock block in _cfg.Blocks)
		{
			foreach (BoundStatement stmt in block.Statements)
			{
				if (stmt is BoundLocalVariableDeclarationStatement decl && decl.Local == variable)
				{
					defBlocks.Add(block);
				}
				else if (stmt is BoundExpressionStatement exprStmt)
				{
					FindAssignmentDefsInExpression(exprStmt.Expression, variable, defBlocks, block);
				}
			}
		}

		return defBlocks;
	}

	private static void FindAssignmentDefsInExpression(BoundExpression expr, LocalVariableOrParameterSymbol variable,
		HashSet<BasicBlock> defBlocks, BasicBlock block)
	{
		if (expr is BoundAssignment assignment)
		{
			if ((assignment.Left is BoundMove move && move.Variable == variable) ||
			    (assignment.Left is BoundCopy copy && copy.Variable == variable))
			{
				defBlocks.Add(block);
			}
			FindAssignmentDefsInExpression(assignment.Right, variable, defBlocks, block);
		}
		else if (expr is BoundBinaryExpression binary)
		{
			FindAssignmentDefsInExpression(binary.Left, variable, defBlocks, block);
			FindAssignmentDefsInExpression(binary.Right, variable, defBlocks, block);
		}
		else if (expr is BoundUnaryExpression unary)
		{
			FindAssignmentDefsInExpression(unary.Expression, variable, defBlocks, block);
		}
		else if (expr is BoundCall call)
		{
			foreach (var arg in call.Arguments)
			{
				FindAssignmentDefsInExpression(arg, variable, defBlocks, block);
			}
		}
	}

	private void AddPhiForVariable(LocalVariableOrParameterSymbol variable, BasicBlock block, FunctionMir mir)
	{
		var mirBlock = mir.Blocks[block];
		TempValue phiOutput = GetNewTemp(variable.Type, variable.Name + ".phi");
		var phi = new PhiInstruction(phiOutput);
		mirBlock.Instructions.Insert(0, phi);

		if (!_phiNodes.ContainsKey(block))
			_phiNodes[block] = new Dictionary<LocalVariableOrParameterSymbol, TempValue>();
		_phiNodes[block][variable] = phiOutput;
	}

	private void RenameAll(FunctionMir mir)
	{
		foreach (var variable in _ssaVariables)
		{
			_versionStack[variable] = new Stack<TempValue>();
		}

		RenameBlock(_cfg.Entry, mir);
	}

	private void RenameBlock(BasicBlock block, FunctionMir mir)
	{
		var mirBlock = mir.Blocks[block];

		var savedHeights = new Dictionary<LocalVariableOrParameterSymbol, int>();
		foreach (var variable in _ssaVariables)
		{
			savedHeights[variable] = _versionStack[variable].Count;
		}

		if (ReferenceEquals(block, _cfg.Entry))
		{
			foreach (var (variable, version) in _initialVersions)
			{
				_versionStack[variable].Push(version);
			}
		}

		if (_phiNodes.TryGetValue(block, out var blockPhis))
		{
			foreach (var (variable, phiOutput) in blockPhis)
			{
				_versionStack[variable].Push(phiOutput);
			}
		}

		foreach (BoundStatement statement in block.Statements)
		{
			RewriteStatement(statement, mirBlock);
		}

		AddPhiIncoming(block, mir);

		Debug.Assert(block.Terminator is not null);
		RewriteTerminator(block.Terminator, mirBlock);

		if (_cfg.DominatorTree.TryGetValue(block, out var children))
		{
			foreach (var child in children)
			{
				RenameBlock(child, mir);
			}
		}

		foreach (var variable in _ssaVariables)
		{
			int target = savedHeights[variable];
			while (_versionStack[variable].Count > target)
				_versionStack[variable].Pop();
		}
	}

	private void AddPhiIncoming(BasicBlock fromBlock, FunctionMir mir)
	{
		foreach (BasicBlock succ in fromBlock.Successors)
		{
			if (!_phiNodes.TryGetValue(succ, out var succPhis))
				continue;

			MirBlock succMir = mir.Blocks[succ];

			foreach (var (variable, phiOutput) in succPhis)
			{
				PhiInstruction? phiInstr = null;
				foreach (var instr in succMir.Instructions)
				{
					if (instr is PhiInstruction p && p.Output == phiOutput)
					{
						phiInstr = p;
						break;
					}
				}

				TempValue incomingValue;
				if (_versionStack.TryGetValue(variable, out var stack) && stack.Count > 0)
				{
					incomingValue = stack.Peek();
				}
				else
				{
					incomingValue = GetNewTemp(variable.Type);
					phiInstr?.Incoming.Add((incomingValue, fromBlock));
					continue;
				}

				phiInstr?.Incoming.Add((incomingValue, fromBlock));
			}
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
				if (_ssaVariables.Contains(declStmt.Local))
				{
					if (declStmt.Initializer != null)
					{
						TempValue value = RewriteExpression(declStmt.Initializer, mirBlock);
						_versionStack[declStmt.Local].Push(value);
					}
				}
				else
				{
					if (declStmt.Initializer != null)
					{
						TempValue value = RewriteExpression(declStmt.Initializer, mirBlock);
						TempValue addr = _addrTable[declStmt.Local];
						mirBlock.Instructions.Add(new StoreInstruction(value, addr));
					}
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
			BoundMove move => EmitLoad(move, block),
			BoundCopy copy => EmitLoad(copy, block),

			_ => throw new UnreachableException($"RewriteExpression({expression.GetType()})")
		};
	}

	private TempValue EmitAddressOfExpression(BoundAddressOfExpression addressOf, MirBlock block)
	{
		TempValue output = GetNewTemp(addressOf.Type);
		LocalVariableOrParameterSymbol symbol = addressOf.Expression switch
		{
			BoundMove move => move.Variable,
			BoundCopy copy => copy.Variable,
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
		if (unary.Op.CorrespondingFunction is not null)
		{
			throw new NotImplementedException("call instruction is not implemented yet");
		}

		TempValue value = RewriteExpression(unary.Expression, block);

		if (unary.Op.Kind == UnaryOperatorKind.Plus)
		{
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
		if (binary.Op.CorrespondingFunction is not null)
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
			_ => throw new UnreachableException($"EmitBinaryExpression({binary.Op.Kind})")
		};

		block.Instructions.Add(instruction);
		return result;
	}

	private TempValue EmitAssignmentExpression(BoundAssignment assignment, MirBlock block)
	{
		TempValue value = RewriteExpression(assignment.Right, block);

		if (assignment.Left is BoundMove move && _ssaVariables.Contains(move.Variable))
		{
			_versionStack[move.Variable].Push(value);
			return value;
		}
		if (assignment.Left is BoundCopy copy && _ssaVariables.Contains(copy.Variable))
		{
			_versionStack[copy.Variable].Push(value);
			return value;
		}

		TempValue addr = GetAssignmentTargetAddress(assignment.Left, block);
		block.Instructions.Add(new StoreInstruction(value, addr));
		return value;
	}

	private TempValue GetAssignmentTargetAddress(BoundExpression expression, MirBlock block)
	{
		return expression switch
		{
			BoundMove move => _addrTable[move.Variable],
			BoundCopy copy => _addrTable[copy.Variable],
			BoundDereferenceExpression dereference => RewriteExpression(dereference.Expression, block),
			_ => throw new UnreachableException($"GetAssignmentTargetAddress({expression.GetType()})")
		};
	}

	private TempValue EmitCall(BoundCall call, MirBlock block)
	{
		TempValue ret = GetNewTemp(call.Function.ReturnType);
		ArrayBuilder<TempValue> arguments = ArrayBuilder<TempValue>.GetInstance();
		foreach (var arg in call.Arguments)
		{
			arguments.Add(RewriteExpression(arg, block));
		}

		block.Instructions.Add(new CallInstruction(ret, call.Function, arguments.ToImmutableAndFree()));

		return ret;
	}

	private TempValue EmitLoad(BoundMove move, MirBlock block)
	{
		if (_ssaVariables.Contains(move.Variable))
		{
			if (_versionStack.TryGetValue(move.Variable, out var stack) && stack.Count > 0)
				return stack.Peek();
			TempValue undef = GetNewTemp(move.Variable.Type);
			block.Instructions.Add(new UndefInstruction(undef));
			return undef;
		}

		TempValue addr = _addrTable[move.Variable];
		TempValue load = GetNewTemp(move.Type);
		block.Instructions.Add(new LoadInstruction(load, addr));
		return load;
	}

	private TempValue EmitLoad(BoundCopy copy, MirBlock block)
	{
		if (_ssaVariables.Contains(copy.Variable))
		{
			if (_versionStack.TryGetValue(copy.Variable, out var stack) && stack.Count > 0)
				return stack.Peek();
			TempValue undef = GetNewTemp(copy.Variable.Type);
			block.Instructions.Add(new UndefInstruction(undef));
			return undef;
		}

		TempValue addr = _addrTable[copy.Variable];
		TempValue load = GetNewTemp(copy.Type);
		block.Instructions.Add(new LoadInstruction(load, addr));
		return load;
	}
}
