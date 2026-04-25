using System.Collections.Generic;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class LookupResult
{
	public LookupResultKind Kind { get; private set; }
	public ArrayBuilder<Symbol> Symbols { get; }
	public Diagnostic? Error { get; private set; }

	private readonly ObjectPool<LookupResult> _pool;

	public bool IsMultiViable => Kind == LookupResultKind.Viable;

	private LookupResult(ObjectPool<LookupResult> pool)
	{
		_pool = pool;
		Kind = LookupResultKind.Empty;
		Symbols = ArrayBuilder<Symbol>.GetInstance();
		Error = null;
	}

	private static readonly ObjectPool<LookupResult> Pool = new(() => new LookupResult(Pool!), 128);

	internal static LookupResult GetInstance()
	{
		LookupResult instance = Pool.Allocate();
		Debug.Assert(instance.IsClear);
		return instance;
	}

	public bool IsClear => Kind == LookupResultKind.Empty && Error == null && Symbols.Count == 0;

	public void Clear()
	{
		Kind = LookupResultKind.Empty;
		Symbols.Clear();
		Error = null;
	}

	public void Free()
	{
		Clear();
		// Symbols.Clear(); reason: object still holds reference to the builder after Free
		_pool.Free(this);
	}

	internal void SetFrom(LookupResult other)
	{
		Kind = other.Kind;
		Symbols.Clear();
		Symbols.AddRange(other.Symbols);
		Error = other.Error;
	}

	internal void SetFrom(SingleLookupResult other)
	{
		Kind = other.Kind;
		Symbols.Clear();
		if (other.Symbol is not null) Symbols.Add(other.Symbol);
		Error = other.Error;
	}

	internal void MergeEqual(LookupResult other)
	{
		if (Kind > other.Kind)
		{
			return;
		}
		else if (other.Kind > Kind)
		{
			SetFrom(other);
		}
		else if (Kind != LookupResultKind.Viable)
		{
			return;
		}
		else
		{
			Symbols.AddRange(other.Symbols);
		}
	}

	internal void MergeEqual(SingleLookupResult other)
	{
		if (Kind > other.Kind)
		{
			return;
		}
		else if (other.Kind > Kind)
		{
			SetFrom(other);
		}
		else if (Kind != LookupResultKind.Viable)
		{
			return;
		}
		else
		{
			Symbols.Add(other.Symbol!);
		}
	}

	/// <summary>
	/// Return the single symbol if there is exactly one, otherwise null.
	/// </summary>
	internal Symbol? SingleSymbolOrDefault => (Symbols.Count == 1) ? Symbols[0] : null;

	public static SingleLookupResult Empty()
	{
		return new SingleLookupResult(LookupResultKind.Empty, null, null);
	}

	public static SingleLookupResult Viable(Symbol result)
	{
		return new SingleLookupResult(LookupResultKind.Viable, result, null);
	}
}
