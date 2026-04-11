using System.Threading;

namespace NiteCompiler.Diagnostics;

internal sealed class BindingDiagnosticBag
{
	private static readonly ObjectPool<BindingDiagnosticBag> BagPool = new(static () => new BindingDiagnosticBag());

	private DiagnosticBag? _bag;

	public static BindingDiagnosticBag GetInstance()
	{
		return BagPool.Allocate();
	}

	public void Free()
	{
		BagPool.Free(this);

		_bag?.Clear();
	}

	public DiagnosticBag Diagnostics
	{
		get
		{
			if (_bag == null)
			{
				Interlocked.CompareExchange(ref _bag, [], null);
			}

			return _bag;
		}
	}

	public DiagnosticBag? ToBagAndFree()
	{
		Free();
		return _bag;
	}
}