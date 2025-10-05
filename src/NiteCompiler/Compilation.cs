using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;
using Binder = NiteCompiler.CodeAnalysis.Binding.Binder;

namespace NiteCompiler;

public sealed class Compilation
{
	public ImmutableArray<SyntaxTree> SyntaxTrees { get; }
	private readonly GlobalSymbolTable _globalSymbolTable;
	public LibrarySymbol Library => _globalSymbolTable.Library;

	public DiagnosticBag Diagnostics { get; }

	public Compilation(string libraryName, params SyntaxTree[] trees)
	{
		SyntaxTrees = [..trees];
		_globalSymbolTable = new(libraryName);
		Diagnostics = [];

		DeclarationPass();
		SignaturePass();

		foreach (ModuleSymbol module in _globalSymbolTable.Modules)
		{
			Console.WriteLine(module.FullName);
			foreach (Symbol symbol in module)
			{
				Console.WriteLine($"\t{symbol}");
			}
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
			{
				if (topLevelNode is ModuleDeclarationSyntax module)
				{
					currentModule = Library.GetOrAddModule(module.Name.GetName());
				}
				else if (topLevelNode is FunctionDeclarationSyntax function)
				{
					FunctionSymbol symbol = new(
						function.Name.GetName(),
						containingSymbol: currentModule,
						function);

					currentModule.AddMember(symbol);
				}
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

            foreach (var function in module.Functions)
            {
                function.ReturnType = ResolveType(function.Syntax.Retusa?.ReturnType);
                foreach (FunctionParameterSyntax paramDecl in function.Syntax.Parameters)
                {
                    var param = new ParameterSymbol(paramDecl.Name.GetName(), function, paramDecl);
                    param.Type = ResolveType(paramDecl.TypeSyntax);
                    function.Parameters.Add(param);
                }
            }
		}
	}

	private TypeSymbol ResolveType(TypeSyntax? typeSyntax)
	{
		throw new NotImplementedException();
		//if typeSyntax is null return _globalSymbolTable.GetDefaultType();
	}

	public void Emit(Stream stream)
	{

	}
}