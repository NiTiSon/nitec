using System.Collections.Generic;
using System.Linq;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Declarations;

internal sealed class TypeDeclaration : Declaration
{
	public string Name { get; }
	public DeclarationModifiers Modifiers { get; }
	private readonly List<TypeDeclarationSyntax> _syntaxes;
	private readonly List<Declaration> _members;
	public IReadOnlyCollection<TypeDeclarationSyntax> Syntaxes => _syntaxes;
	public IEnumerable<SyntaxNode> Names => _syntaxes.Select(t => t.Name);
	public IReadOnlyCollection<Declaration> Members => _members;


	public TypeDeclaration(string name, TypeDeclarationSyntax syntax, DeclarationModifiers modifiers)
	{
		Modifiers = modifiers;
		Name = name;
		_syntaxes = [syntax];
		_members = [];
	}

	public void AddSyntax(TypeDeclarationSyntax syntax)
	{
		_syntaxes.Add(syntax);
	}

	public void AddMember(Declaration member)
	{
		_members.Add(member);
	}
}