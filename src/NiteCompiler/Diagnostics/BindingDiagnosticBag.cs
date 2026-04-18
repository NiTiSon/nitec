using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

	public bool IsEmpty => _bag == null || _bag.IsEmpty;

	public void Add(Diagnostic diagnostic)
	{
		Diagnostics.Add(diagnostic);
	}

	public void AddRange(ImmutableArray<Diagnostic> diagnostics)
	{
		foreach (var diagnostic in diagnostics)
		{
			Add(diagnostic);
		}
	}

	public void AddRange(ReadOnlySpan<Diagnostic> diagnostics)
	{
		foreach (var diagnostic in diagnostics)
		{
			Add(diagnostic);
		}
	}

	public void AddRange(IEnumerable<Diagnostic> diagnostics)
	{
		foreach (var diagnostic in diagnostics)
		{
			Add(diagnostic);
		}
	}

	public DiagnosticBag? ToBagAndFree()
	{
		var bag = _bag;
		_bag = null;
		BagPool.Free(this);
		return bag;
	}

	public ImmutableArray<Diagnostic> ToImmutableAndFree()
	{
		var bag = _bag;
		_bag = null;
		BagPool.Free(this);
		return bag?.ToImmutableArray() ?? [];
	}
}
