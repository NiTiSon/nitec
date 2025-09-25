namespace NiteCompiler.CodeAnalysis.Binding;

internal class BoundScope
{
	private BoundScope? _parent;

	public BoundScope(BoundScope? parent = null)
	{
		_parent = parent;
	}
}