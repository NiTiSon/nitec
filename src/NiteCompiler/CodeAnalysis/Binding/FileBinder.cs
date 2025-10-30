using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class FileBinder : BinderWithUsagesAndAliases
{
	public FileBinder(Binder? parent, Compilation compilation, IEnumerable<UseDirectiveSyntax> usages) : base(parent, compilation, usages)
	{
	}
}