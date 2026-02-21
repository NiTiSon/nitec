using System.Collections.Generic;
using System.Collections.Immutable;

namespace NiteCompiler.CodeAnalysis.Declarations;

internal abstract class MergedItemDeclaration : Declaration
{
	protected MergedItemDeclaration(string name) : base(name)
	{
	}
}