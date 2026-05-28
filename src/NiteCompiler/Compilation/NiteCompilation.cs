using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using LLVMSharp.Interop;
using NiteCompiler.CodeAnalysis.Declarations;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Dependencies;
using NiteCompiler.Diagnostics;
using NiteCompiler.Metadata;
using NiteCompiler.Emitting;

namespace NiteCompiler.Compilation;

public sealed partial class NiteCompilation
{
	private const string FallbackLibraryName = "unnamed-library";

	internal DeclarationTable Declarations { get; }
	internal MergedModuleDeclaration MergedRoot => Declarations.GetMergedRoot(this);

	public ImmutableArray<SyntaxTree> SyntaxTrees { get; }
	public NiteCompilationOptions Options { get; }
	internal SourceLibrarySymbol SourceLibrary { get; }

	internal BuiltInOperators BuiltInOperators
	{
		get
		{
			if (field == null)
			{
				Interlocked.CompareExchange(ref field, new BuiltInOperators(this), null);
			}

			return field;
		}
	}

	private NiteCompilation(
		string libraryName,
		ImmutableArray<SyntaxTree> syntaxTrees,
		ImmutableArray<Dependency> dependencies,
		NiteCompilationOptions options,
		DiagnosticBag diagnostics)
	{
		SyntaxTrees = syntaxTrees;
		Options = options;

		if (Options.IsCoreLibrary && !dependencies.IsEmpty)
		{
			diagnostics.ReportDependenciesInCoreLibrary();
		}

		Declarations = new(syntaxTrees);

		SourceLibrary = new(this, libraryName);
	}

	public static NiteCompilation Create(
		string libraryName,
		IEnumerable<SyntaxTree>? sourceFiles,
		IEnumerable<Dependency>? dependencies,
		NiteCompilationOptions options,
		DiagnosticBag diagnostics)
	{
		ImmutableArray<SyntaxTree> syntaxTrees = sourceFiles?.ToImmutableArray() ?? [];
		ImmutableArray<Dependency> dependenciesArray = dependencies?.ToImmutableArray() ?? [];

		libraryName ??= FallbackLibraryName;
		return new NiteCompilation(libraryName, syntaxTrees, dependenciesArray, options, diagnostics);
	}

	internal int GetSyntaxTreeOrdinal(SyntaxTree tree)
	{
		// TODO: Improve
		int i;
		for (i = 0; i < SyntaxTrees.Length; i++)
		{
			if (tree == SyntaxTrees[i])
			{
				return i;
			}
		}

		throw new KeyNotFoundException($"SyntaxTree is not used within this compilation: '{tree.FilePath ?? "empty-filepath"}'");
	}

	internal bool LookingForSpecialTypes
	{
		get
		{
			if (_lateinitSpecialTypes == null) return true;

			for (int i = 1; i < _lateinitSpecialTypes.Length; i++)
			{
				if (_lateinitSpecialTypes[i] is null) return true;
			}

			return false;
		}
	}

	private TypeSymbol?[]? _lateinitSpecialTypes = null;
	internal void RegisterSpecialType(TypeSymbol type)
	{
		if (_lateinitSpecialTypes == null)
		{
			Interlocked.CompareExchange(ref _lateinitSpecialTypes, new TypeSymbol[(int)SpecialType.Count], null);
		}

		_lateinitSpecialTypes[(int)type.SpecialType] = type;
	}

	public TypeSymbol GetSpecialType(SpecialType type)
	{
		if (type == SpecialType.None || (int)SpecialType.Count <= (int)type)
		{
			throw new ArgumentOutOfRangeException(nameof(type), $"Unexpected SpecialType: '{type}'.");
		}

		TypeSymbol result;
		if (IsMissingType(type))
		{
			string fullName = type.ToFullName();
			DeclarationDiagnostics.ReportUnresolvedPredefinedType(fullName);

			result = new ErrorTypeSymbol(this, type, fullName, 0, 0, null, unreported: false);
		}
		else
		{
			result = _lateinitSpecialTypes![(int)type]!;
		}

		Debug.Assert(result.SpecialType == type);
		return result;
	}

	private bool IsMissingType(SpecialType type)
	{
		return _lateinitSpecialTypes?[(int)type] is null;
	}

	public LifetimeSymbol GetStaticLifetime() => StaticLifetimeSymbol.Instance;

	private ConcurrentDictionary<(TypeSymbol, bool, bool, LifetimeSymbol?), ReferenceTypeSymbol>? _referenceTypeSymbols;
	internal ReferenceTypeSymbol CreateReferenceType(TypeSymbol pointsTo, bool isMutable, bool isNullable, LifetimeSymbol? lifetime = null)
	{
		if (_referenceTypeSymbols == null)
		{
			Interlocked.CompareExchange(ref _referenceTypeSymbols, new(), null);
		}

		if (!_referenceTypeSymbols.TryGetValue((pointsTo, isMutable, isNullable, lifetime), out ReferenceTypeSymbol? value))
		{
			value = new ReferenceTypeSymbol(pointsTo, isMutable, isNullable, lifetime);
			_referenceTypeSymbols[(pointsTo, isMutable, isNullable, lifetime)] = value;
		}

		return value;
	}

	private ConcurrentDictionary<(TypeSymbol, bool, bool), PointerTypeSymbol>? _pointerTypeSymbols;
	internal PointerTypeSymbol CreatePointerType(TypeSymbol pointsTo, bool isMutable, bool isNullable)
	{
		if (_pointerTypeSymbols == null)
		{
			Interlocked.CompareExchange(ref _pointerTypeSymbols, new(), null);
		}

		if (!_pointerTypeSymbols.TryGetValue((pointsTo, isMutable, isNullable), out PointerTypeSymbol? value))
		{
			value = new PointerTypeSymbol(pointsTo, isMutable, isNullable);
			_pointerTypeSymbols[(pointsTo, isMutable, isNullable)] = value;
		}

		return value;
	}

	private ConcurrentDictionary<TypeSymbol, UnsizedArrayTypeSymbol>? _unsizedArrayTypeSymbols;
	internal UnsizedArrayTypeSymbol CreateUnsizedArrayType(TypeSymbol elementsType)
	{
		if (_unsizedArrayTypeSymbols == null)
		{
			Interlocked.CompareExchange(ref _unsizedArrayTypeSymbols, new(), null);
		}

		if (!_unsizedArrayTypeSymbols.TryGetValue(elementsType, out UnsizedArrayTypeSymbol? value))
		{
			value = new UnsizedArrayTypeSymbol(elementsType);
			_unsizedArrayTypeSymbols[elementsType] = value;
		}

		return value;
	}

	private ConcurrentDictionary<(TypeSymbol, ulong), SizedArrayTypeSymbol>? _sizedArrayTypeSymbols;
	internal SizedArrayTypeSymbol CreateSizedArrayType(TypeSymbol elementsType, ulong length)
	{
		if (_sizedArrayTypeSymbols == null)
		{
			Interlocked.CompareExchange(ref _sizedArrayTypeSymbols, new(), null);
		}

		var key = (elementsType, length);
		if (!_sizedArrayTypeSymbols.TryGetValue(key, out SizedArrayTypeSymbol? value))
		{
			value = new SizedArrayTypeSymbol(elementsType, length);
			_sizedArrayTypeSymbols[key] = value;
		}

		return value;
	}

	public FunctionSymbol? GetEntryPoint()
	{
		// TODO: Binder.Lookup("main") etc.
		return null;
	}

	public (LLVMModuleRef, LLVMTargetMachineRef, LLVMTargetDataRef) GetLlvmModule(out DiagnosticBag? resultDiagnostics, [NotNull] ref string? targetTriple)
	{
		LLVM.InitializeAllTargetInfos();
		LLVM.InitializeAllTargets();
		LLVM.InitializeAllTargetMCs();
		LLVM.InitializeAllAsmParsers();
		LLVM.InitializeAllAsmPrinters();

		if (string.IsNullOrEmpty(targetTriple))
		{
			targetTriple = LLVMTargetRef.DefaultTriple;
		}

		BindingDiagnosticBag diagnostics = BindingDiagnosticBag.GetInstance();
		if (LLVMTargetRef.TryGetTargetFromTriple(targetTriple, out LLVMTargetRef target, out string error))
		{
			LLVMTargetMachineRef machine = target.CreateTargetMachine(
				targetTriple,
				"generic",
				[],
				LLVMCodeGenOptLevel.LLVMCodeGenLevelDefault,
				LLVMRelocMode.LLVMRelocDefault,
				LLVMCodeModel.LLVMCodeModelDefault);
			LLVMTargetDataRef data = machine.CreateTargetDataLayout();

			LLVMModuleRef module = LlvmTranslator.Translate(this, [SourceLibrary], GetEntryPoint(), diagnostics);
			module.Target = targetTriple;
			LlvmOptimizer.Optimize(module, machine);
			resultDiagnostics = diagnostics.ToBagAndFree();
			return (module, machine, data);
		}
		Debug.WriteLine(error);

		resultDiagnostics = diagnostics.ToBagAndFree();
		return default;
	}

	public void EmitNiteLibrary(Stream stream, out DiagnosticBag? resultDiagnostics)
	{
		BindingDiagnosticBag diagnostics = BindingDiagnosticBag.GetInstance();
		MetadataLibraryBuilder.Translate(this, stream, diagnostics);

		resultDiagnostics = diagnostics.ToBagAndFree();
	}
}
