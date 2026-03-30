using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.CodeAnalysis.Binding;

internal class LocalScopeBinder : Binder
{
	private ImmutableArray<LocalVariableSymbol> _locals;

	public LocalScopeBinder(Binder parent) : base(parent)
	{
	}

	public LocalScopeBinder(Binder parent, BinderFlags flags) : base(parent, flags)
	{
	}

	public sealed override ImmutableArray<LocalVariableSymbol> Locals
	{
		get
		{
			if (_locals.IsDefault)
			{
				ImmutableInterlocked.InterlockedCompareExchange(ref _locals, BuildLocals(), default);
			}

			return _locals;
		}
	}

	protected virtual ImmutableArray<LocalVariableSymbol> BuildLocals()
	{
		return [];
	}
}