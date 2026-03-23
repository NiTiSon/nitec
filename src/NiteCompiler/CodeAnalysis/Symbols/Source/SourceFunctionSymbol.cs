using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Binding.BoundTree;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceFunctionSymbol : FunctionSymbol
{
	public override Symbol ContainingSymbol { get; }
	public FunctionDeclarationSyntax Syntax { get; }
	public override string Name => Syntax.Name.GetName();

	public SourceFunctionSymbol(Symbol containingSymbol, FunctionDeclarationSyntax syntax)
	{
		Debug.Assert(containingSymbol != null);

		ContainingSymbol = containingSymbol;
		Syntax = syntax;
	}

	public override void Accept(SymbolVisitor visitor)
	{
		visitor.VisitFunction(this);
	}

	public override TResult? Accept<TResult>(SymbolVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitFunction(this);
	}

	public override TResult? Accept<TResult, TArgument>(SymbolVisitor<TResult, TArgument> visitor, TArgument arg) where TResult : default
	{
		return visitor.VisitFunction(this, arg);
	}

	private FunctionBodySyntax GetInFunctionSyntaxNode()
	{
		return Syntax.Body;
	}

	public Binder? TryGetInFunctionBinder(BinderFactory? binderFactory = null)
	{
		SyntaxNode inNode = GetInFunctionSyntaxNode();

		Binder result = (binderFactory ?? DeclaringCompilation!.GetBinderFactory(inNode.Tree)).GetBinder(inNode);
#if DEBUG
		Binder? current = result;
		do
		{
			if (current is InFunctionBinder)
			{
				break;
			}

			current = current.Parent;
		}
		while (current != null);

		Debug.Assert(current is InFunctionBinder);
#endif
		return result;
	}

	public ExecutableCodeBinder? TryGetBodyBinder()
	{
		Binder? inFunctionBinder = TryGetInFunctionBinder();
		FunctionBodySyntax body = GetInFunctionSyntaxNode();
		SyntaxNode? syntax = null;
		if (body is BlockFunctionBodySyntax blockBody)
		{
			syntax = blockBody.Block;
		}

		Debug.Assert(syntax != null);
		return inFunctionBinder == null ? null : new ExecutableCodeBinder(syntax, this, inFunctionBinder);
	}
}