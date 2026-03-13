using System.Collections.Generic;
using System.Threading;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class ExecutableCodeBinder : Binder
{
	private readonly Symbol _memberSymbol;
	private readonly SyntaxNode _root;

	public ExecutableCodeBinder(SyntaxNode root, Symbol memberSymbol, Binder parent) : base(parent)
	{
		_memberSymbol = memberSymbol;
		_root = root;
	}

	public override Binder GetBinder(SyntaxNode node)
	{
		return BinderMap.TryGetValue(node, out Binder? binder)
			? binder
			: Parent.GetBinder(node);
	}

	private void ComputeBinderMap()
	{
		Dictionary<SyntaxNode, Binder> map;

		if (_memberSymbol != null && _root != null)
		{
			map = [];
		}
		else
		{
			map = [];
		}

		Interlocked.CompareExchange(ref _lazyBinderMap, map, null);
	}

	private Dictionary<SyntaxNode, Binder>? _lazyBinderMap;
	private Dictionary<SyntaxNode, Binder> BinderMap
	{
		get
		{
			if (_lazyBinderMap == null)
			{
				ComputeBinderMap();
			}

			return _lazyBinderMap!;
		}
	}
}