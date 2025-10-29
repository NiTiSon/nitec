using System;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class FileBinder : Binder
{
	public SyntaxTree Tree { get; }

	public FileBinder(Binder? parent, SyntaxTree tree,  GlobalScope globalScope, ModuleManager moduleManager,
		params ReadOnlySpan<UseDirectiveSyntax> usings) : base(parent, globalScope, moduleManager, usings)
	{
		Tree = tree;
	}


}