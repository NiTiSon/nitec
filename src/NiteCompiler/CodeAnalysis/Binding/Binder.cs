using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class Binder
{
	private readonly BoundScope _scope;
	private readonly FunctionSymbol _function;

	private Binder(BoundScope? parent, FunctionSymbol function)
	{
		_scope = new BoundScope(parent);
		_function = function;

		// if (function != null)
		// {
		// 	foreach (var p in function.Parameters)
		// 		_scope.TryDeclareVariable(p);
		// }
	}
}