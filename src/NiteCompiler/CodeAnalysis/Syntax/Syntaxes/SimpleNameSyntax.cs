using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class SimpleNameSyntax : NameSyntax
{
	private readonly string _name;

	private protected SimpleNameSyntax(SyntaxTree tree, string identifier) : base(tree)
	{
		_name = identifier;
	}

	public sealed override SimpleNameSyntax UnqualifiedName => this;

	public override string GetName()
	{
		return _name;
	}
}