using System.Collections.Generic;
using System.Linq;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Declarations;

internal sealed class FileDeclaration : ContainingDeclaration
{
	private readonly List<UseOrUseAsDirectiveSyntax> _directives = [];
	private readonly List<Declaration> _members = [];

	public SyntaxTree SyntaxTree { get; }

	public IReadOnlyCollection<UseOrUseAsDirectiveSyntax> Directives => _directives;
	public IEnumerable<UseDirectiveSyntax> Usages => _directives.OfType<UseDirectiveSyntax>();
	// public IEnumerable<object?> Aliases => _useDirectives.OfType<object?>();
	public IReadOnlyCollection<Declaration> Members => _members;

	public FileDeclaration(SyntaxTree syntaxTree)
	{
		SyntaxTree = syntaxTree;
	}

	public void AddDirective(UseOrUseAsDirectiveSyntax directive)
	{
		_directives.Add(directive);
	}

	public override void AddMember(Declaration declaration)
	{
		_members.Add(declaration);
	}
}