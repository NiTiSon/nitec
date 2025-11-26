using System;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class ModuleBinder : Binder
{
	public ModuleSymbol Module { get; }

	public ModuleBinder(Compilation compilation, Binder parent, ModuleSymbol module) : base(compilation, parent)
	{
		Module = module;
	}

	protected override Symbol? LookupSymbol(string name, LookupOptions options = LookupOptions.Default)
	{
		return LookupSymbolInParent(name, options);
	}

	public override BoundNode Bind(SyntaxNode syntax)
	{
		return syntax switch
		{
			// FunctionDeclarationSyntax func => BindFunctionDeclaration(func),
			// TypeDeclarationSyntax type => BindTypeDeclaration(type),
			_ => throw new ArgumentException(null, nameof(syntax))
		};
	}

	// private BoundNode BindFunctionDeclaration(FunctionDeclarationSyntax syntax)
	// {
	// 	FunctionBinder binder = new(Compilation, this, (Compilation.GetSymbol(syntax) as FunctionSymbol)!);
	// 	return binder.Bind(syntax);
	// }
	//
	// private BoundNode BindTypeDeclaration(TypeDeclarationSyntax syntax)
	// {
	// 	TypeBinder binder = new(Compilation, this, (Compilation.GetSymbol(syntax) as TypeSymbol)!);
	// 	return binder.Bind(syntax);
	// }
}