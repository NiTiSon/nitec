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

	public Compilation(string libraryName, params SyntaxTree[] trees)
	{
		SyntaxTrees = [..trees];
		_globalSymbolTable = new(libraryName);
		Diagnostics = [];

		DeclarationPass();
		SignaturePass();

		foreach (ModuleSymbol module in _globalSymbolTable.Modules)
		{
			Console.WriteLine(module.Name);
			foreach (Symbol symbol in module) Console.WriteLine($"\t{symbol}");
		}
	}

	public ImmutableArray<SyntaxTree> SyntaxTrees { get; }
	public LibrarySymbol Library => _globalSymbolTable.Library;

	public DiagnosticBag Diagnostics { get; }

	/// <summary>
	///     Save only declarations, do not work with arguments, parameters, typization, and the other shit yet.
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

	private void SignaturePass()
	{
		foreach (ModuleSymbol module in Library.Modules)
		{
			// foreach (TypeSymbol type in module.Types)
			//          {
			//              // base type resolution
			//              if (type.DeclarationSyntax.BaseType is { } baseTypeSyntax)
			//              {
			//                  var resolved = ResolveType(baseTypeSyntax);
			//                  if (resolved is null)
			//                      Diagnostics.ReportError($"Unknown base type: {baseTypeSyntax}");
			//                  else
			//                      type.BaseType = resolved;
			//              }
			//
			//              // fields
			//              foreach (var fieldDecl in type.DeclarationSyntax.Members.OfType<FieldDeclarationSyntax>())
			//              {
			//                  var field = new FieldSymbol(fieldDecl.Name.GetName(), type, fieldDecl);
			//                  field.Type = ResolveType(fieldDecl.TypeSyntax);
			//                  type.Fields.Add(field);
			//              }
			//
			//              // methods
			//              foreach (var methodDecl in type.DeclarationSyntax.Members.OfType<FunctionDeclarationSyntax>())
			//              {
			//                  var method = new FunctionSymbol(methodDecl.Name.GetName(), type, methodDecl);
			//                  method.ReturnType = ResolveType(methodDecl.ReturnTypeSyntax);
			//                  foreach (var paramDecl in methodDecl.Parameters)
			//                  {
			//                      var param = new ParameterSymbol(paramDecl.Name.GetName(), method, paramDecl);
			//                      param.Type = ResolveType(paramDecl.TypeSyntax);
			//                      method.Parameters.Add(param);
			//                  }
			//                  type.Methods.Add(method);
			//              }
			//          }
			foreach (FunctionSymbol function in module.Functions)
			{
				function.ReturnType = ResolveType(function.Syntax.Retusa?.ReturnType);
				int index = 0;

				ImmutableArray<ParameterSymbol>.Builder parameters = ImmutableArray.CreateBuilder<ParameterSymbol>();
				foreach (FunctionParameterSyntax paramDecl in function.Syntax.Parameters)
				{
					ParameterSymbol param = new(function, paramDecl.Name.GetName(), index++, paramDecl);

					param.Type = ResolveType(paramDecl.TypeClause.Type);
					parameters.Add(param);
				}

				function.Parameters = parameters.ToImmutable();
			}
		}
	}

	private TypeSymbol ResolveType(TypeSyntax typeSyntax)
	{
		if (typeSyntax is NameWithExplicitModuleSyntax nameWithExplicitModuleSyntax)
		{
			ModuleSymbol module = _globalSymbolTable.GetCombinedModule(nameWithExplicitModuleSyntax.GetModuleName());

			TypeSymbol? type = module.Members.FirstOrDefault(t =>
				t is TypeSymbol type && type.Name == nameWithExplicitModuleSyntax.GetName()) as TypeSymbol;

			if (type is null)
				Diagnostics.ReportUnresolvedSymbol(typeSyntax.Span.Contextualize(typeSyntax.SyntaxTree.Text));

			return type;
		}

		if (typeSyntax is PredefinedTypeSyntax predefinedTypeSyntax)
			return _globalSymbolTable.GetPredefinedType(
				SyntaxFacts.GetDefaultTypeByToken(predefinedTypeSyntax.Keyword));

		throw new NotImplementedException();
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