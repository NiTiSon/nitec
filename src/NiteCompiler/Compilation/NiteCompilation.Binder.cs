using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.Compilation;

public sealed partial class NiteCompilation
{
	private WeakReference<BinderFactory>?[]? _binderFactories;

	internal BinderFactory GetBinderFactory(SyntaxTree syntaxTree)
	{
		return GetBinderFactory(syntaxTree, ref _binderFactories);
	}

	private BinderFactory GetBinderFactory(SyntaxTree syntaxTree,
		ref WeakReference<BinderFactory>?[]? cachedBinderFactories)
	{
		var treeNum = GetSyntaxTreeOrdinal(syntaxTree);
		WeakReference<BinderFactory>?[]? binderFactories = cachedBinderFactories;
		if (binderFactories == null)
		{
			binderFactories = new WeakReference<BinderFactory>[this.SyntaxTrees.Length];
			binderFactories = Interlocked.CompareExchange(ref cachedBinderFactories, binderFactories, null) ??
			                  binderFactories;
		}

		var previousWeakReference = binderFactories[treeNum];
		if (previousWeakReference != null && previousWeakReference.TryGetTarget(out BinderFactory? previousFactory))
		{
			return previousFactory;
		}

		return AddNewFactory(syntaxTree, ref binderFactories[treeNum]);
	}

	private BinderFactory AddNewFactory(SyntaxTree syntaxTree, [NotNull] ref WeakReference<BinderFactory>? slot)
	{
		var newFactory = new BinderFactory(this, syntaxTree);
		var newWeakReference = new WeakReference<BinderFactory>(newFactory);

		while (true)
		{
			WeakReference<BinderFactory>? previousWeakReference = slot;
			if (previousWeakReference != null && previousWeakReference.TryGetTarget(out BinderFactory? previousFactory))
			{
				Debug.Assert(slot != null);
				return previousFactory;
			}

			if (Interlocked.CompareExchange(ref slot!, newWeakReference, previousWeakReference) ==
			    previousWeakReference)
			{
				return newFactory;
			}
		}
	}

	internal Binder GetBinder(SyntaxNode node)
	{
		return GetBinderFactory(node.Tree).GetBinder(node);
	}
}