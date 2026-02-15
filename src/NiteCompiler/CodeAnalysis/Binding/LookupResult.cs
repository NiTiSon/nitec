using System.Collections.Generic;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class LookupResult
{
	public LookupResultKind Kind { get; private set; }
	public List<Symbol> SymbolList { get; }
	public Diagnostic? Error { get; private set; }

	private readonly ObjectPool<LookupResult> _pool;

	private LookupResult(ObjectPool<LookupResult> pool)
	{
		_pool = pool;
		Kind = LookupResultKind.Empty;
		SymbolList = [];
		Error = null;
	}

	private static readonly ObjectPool<LookupResult> pool = new(() => new LookupResult(pool!), 128);

	internal static LookupResult GetInstance()
	{
		LookupResult instance = pool.Allocate();
		Debug.Assert(instance.IsClear);
		return instance;
	}

	public bool IsClear => Kind == LookupResultKind.Empty && Error == null && SymbolList.Count == 0;

	public void Clear()
	{
		Kind = LookupResultKind.Empty;
		SymbolList.Clear();
		Error = null;
	}

	public void Free()
	{
		this.Clear();
		_pool.Free(this);
	}

	/// <summary>
	/// Return the single symbol if there is exactly one, otherwise null.
	/// </summary>
	internal Symbol? SingleSymbolOrDefault => (SymbolList.Count == 1) ? SymbolList[0] : null;
}