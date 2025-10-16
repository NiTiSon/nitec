using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler;

public sealed class Compilation
{
	private readonly GlobalSymbolTable _globalSymbolTable;
	public ImmutableArray<SyntaxTree> SyntaxTrees { get; }
	public LibrarySymbol Library => _globalSymbolTable.Library;
	public DiagnosticBag Diagnostics { get; }

	public Compilation(string libraryName, params SyntaxTree[] trees)
	{
		SyntaxTrees = [..trees];
		_globalSymbolTable = new(libraryName);
		Diagnostics = [];

		Binder binder = new(this);
		DeclarationPass();
		_globalSymbolTable.ResolvePredefinedTypes();
		// binder.BindSignatures();
		// binder.BindBodies();

		_globalSymbolTable.Diagnostics.DrainInto(Diagnostics);

		foreach (ModuleSymbol module in _globalSymbolTable.Modules)
		{
			Console.WriteLine(module.Name);
			foreach (Symbol symbol in module) Console.WriteLine($"\t{symbol}");
		}
	}

	/// <summary>
	/// Save only declarations, do not work with arguments, parameters, typization, and the other shit yet.
	/// </summary>
	private void DeclarationPass()
	{
		foreach (SyntaxTree tree in SyntaxTrees)
		{
			ModuleSymbol currentModule = Library.GetOrAddModule(null);
			foreach (SyntaxNode topLevelNode in tree.Root.TopLevelNodes)
				if (topLevelNode is ModuleDeclarationSyntax module)
				{
					currentModule = Library.GetOrAddModule(module.Name.GetName());
				}
				else if (topLevelNode is FunctionDeclarationSyntax function)
				{
					FunctionSymbol symbol = new(
						function.Name.GetName(),
						currentModule,
						function);

					currentModule.AddMember(symbol);
				}
				else if (topLevelNode is TypeDeclarationSyntax type)
				{
					NamedTypeSymbol symbol = new(
						type.Name.GetName(),
						currentModule);

					currentModule.AddMember(symbol);
				}
				else if (topLevelNode is FieldDeclarationSyntax field)
				{
					FieldSymbol symbol = new(
						field.Name.GetName(),
						currentModule,
						field);

					if (field.TypeClause == null && field.Initializer == null)
						Diagnostics.ReportFieldMustHaveEitherTypeClauseOrDefaultValue(field.ContextualizedSpan);

					currentModule.AddMember(symbol);
				}
		}
	}

	// TODO: resolve default values in BodyPass, not in previous SignaturePass
	private void BodyPass()
	{
		foreach (var module in _globalSymbolTable.Library.Modules)
		{
			foreach (var function in module.Functions)
			{
				BindBody(function, function.Syntax.Block);
			}
		}
	}

	private void BindBody(FunctionSymbol function, BlockStatementSyntax body)
	{

	}

	public void Emit(Stream stream)
	{
	}
}