using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class InFunctionBinder : Binder
{
	private readonly FunctionSymbol _owner;

	public InFunctionBinder(FunctionSymbol owner, Binder parent) : base(parent)
	{
		_owner = owner;
	}

	public override Symbol ContainingMember => _owner;
}