using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using LLVMSharp;
using NiteCompiler.CodeAnalysis.Declarations;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.Compilation;

internal sealed class SymbolPass
{
	public NiteCompilation Compilation { get; }
	private DeclarationPass DeclarationPass { get; }
	public DiagnosticBag Diagnostics { get; } = [];
	private ImmutableArray<TypeSymbol> _specialTypes;
	public SourceLibrarySymbol LibrarySymbol { get; }

	public SymbolPass(NiteCompilation compilation, string libraryName, DeclarationPass declarationPass)
	{
		Compilation = compilation;
		DeclarationPass = declarationPass;

		// Symbol initialization
		{
			LibrarySymbol = new(libraryName);
			LibrarySymbol.InitializeModules(BuildModules(LibrarySymbol));
		}

		_specialTypes = [];
	}

	private ImmutableArray<SourceModuleSymbol> BuildModules(SourceLibrarySymbol librarySymbol)
	{
		var allModulesBuilder = ImmutableArray.CreateBuilder<SourceModuleSymbol>();

		var globalModule = new SourceModuleSymbol(MetadataFacts.GlobalModuleInternalName, librarySymbol);
		var membersBuilder = ImmutableArray.CreateBuilder<Symbol>();
		InitializeModuleMembers(globalModule, ref membersBuilder, DeclarationPass.Files
			.SelectMany(t => t.Members)
			.Where(t => t is not ModuleDeclaration));
		globalModule.InitializeMembers(membersBuilder.DrainToImmutable());
		allModulesBuilder.Add(globalModule);

		foreach (ModuleDeclaration declaration in DeclarationPass.Modules)
		{
			SourceModuleSymbol module = new(librarySymbol, declaration);
			InitializeModuleMembers(module, ref membersBuilder, declaration.Members);
			module.InitializeMembers(membersBuilder.DrainToImmutable());
			allModulesBuilder.Add(module);
		}

		return allModulesBuilder.ToImmutableArray();
	}

	private void InitializeModuleMembers(SourceModuleSymbol containing, ref ImmutableArray<Symbol>.Builder builder,
		IEnumerable<Declaration> memberDeclaration)
	{
		foreach (Declaration declaration in memberDeclaration)
		{
			switch (declaration)
			{
				case FunctionDeclaration functionDeclaration:
				{
					SourceFunctionSymbol function = new(containing, functionDeclaration);
					builder.Add(function);
					break;
				}
				case TypeDeclaration typeDeclaration:
				{
					SourceTypeSymbol type = new(containing, typeDeclaration);
					builder.Add(type);
					break;
				}
				case FieldDeclaration fieldDeclaration:
				{
					// TODO
					break;
				}
			}
		}
	}
}