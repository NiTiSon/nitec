using System.Collections.Generic;
using System.Linq;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Declarations;

internal sealed class ModuleDeclaration : ContainingDeclaration
{
	private readonly List<ModuleDeclarationSyntax> _syntaxes;
	private readonly List<Declaration> _members = [];

	public string Name { get; }
	public IReadOnlyCollection<ModuleDeclarationSyntax> Syntaxes => _syntaxes;
	public IEnumerable<SyntaxNode> Names => _syntaxes.Select(t => t.Name);
	public IReadOnlyCollection<Declaration> Members => _members;

	public ModuleDeclaration(string name, ModuleDeclarationSyntax syntax)
	{
		Name = name;
		_syntaxes = [syntax];
	}

	public void AddSyntax(ModuleDeclarationSyntax syntax)
	{
		_syntaxes.Add(syntax);
	}

	public override void AddMember(Declaration declaration)
	{
		_members.Add(declaration);
	}
}