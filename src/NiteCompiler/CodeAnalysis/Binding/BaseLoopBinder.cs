namespace NiteCompiler.CodeAnalysis.Binding;

internal abstract class BaseLoopBinder : LocalScopeBinder
{
	protected BaseLoopBinder(Binder parent) : base(parent)
	{
	}
}