using System.Collections.Generic;
using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal abstract class BinderWithUsagesAndAliases : Binder
{
	protected ImmutableArray<UseDirectiveSyntax> Usages { get; }

	protected BinderWithUsagesAndAliases(Binder? parent, Compilation compilation, IEnumerable<UseDirectiveSyntax> usages) : base(parent, compilation)
	{
		Usages = [..usages];
	}
}