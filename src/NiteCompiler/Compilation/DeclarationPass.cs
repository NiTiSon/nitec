using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using NiteCompiler.CodeAnalysis.Declarations;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.Compilation;

internal sealed class DeclarationPass
{
	private readonly Dictionary<string, ModuleDeclaration> _modules = [];
	private readonly Dictionary<SyntaxTree, FileDeclaration> _files;

	public NiteCompilation Compilation { get; }
	public DiagnosticBag Diagnostics { get; } = [];
	public IEnumerable<ModuleDeclaration> Modules => _modules.Values;
	public IEnumerable<FileDeclaration> Files => _files.Values;

	public DeclarationPass(NiteCompilation compilation, ImmutableArray<SyntaxTree> syntaxTree)
	{
		Compilation = compilation;
		_files = new();
		_files.EnsureCapacity(syntaxTree.Length);

		foreach (var tree in syntaxTree)
		{
			FileDeclaration file;
			_files[tree] = file = new(tree);
			ContainingDeclaration currentContainerDeclaration = file;
			foreach (SyntaxNode rootNode in tree.Root.TopLevelNodes)
			{
				switch (rootNode)
				{
					case UseOrUseAsDirectiveSyntax directive:
						file.AddDirective(directive);
						break;
					case ModuleDeclarationSyntax moduleDeclaration:
					{
						ModuleDeclaration declaration = AddModuleDeclaration(moduleDeclaration);
						file.AddMember(declaration);
						currentContainerDeclaration = declaration;
						break;
					}
					case FunctionDeclarationSyntax functionDeclaration:
					{
						FunctionDeclaration declaration = AddFunction(functionDeclaration);
						currentContainerDeclaration.AddMember(declaration);
						break;
					}
					case FieldDeclarationSyntax fieldDeclaration:
					{
						FieldDeclaration declaration = AddField(fieldDeclaration);
						currentContainerDeclaration.AddMember(declaration);
						break;
					}
					case TypeDeclarationSyntax typeDeclaration:
					{
						TypeDeclaration declaration = AddType(typeDeclaration);
						currentContainerDeclaration.AddMember(declaration);
						break;
					}
				}
			}
		}
	}

	public FileDeclaration GetFileDeclaration(SyntaxTree syntaxTree)
	{
		return _files[syntaxTree];
	}

	private ModuleDeclaration AddModuleDeclaration(ModuleDeclarationSyntax syntax)
	{
		string name = syntax.Name.GetName();
		ref ModuleDeclaration? declaration = ref CollectionsMarshal.GetValueRefOrAddDefault(_modules, name, out bool exists);

		if (exists)
		{
			declaration!.AddSyntax(syntax);
		}
		else
		{
			declaration = new(name, syntax);
		}

		return declaration;
	}

	private FunctionDeclaration AddFunction(FunctionDeclarationSyntax syntax)
	{
		string name = syntax.Name.GetName();

		DeclarationModifiers modifiers = DeclarationFacts.ModifiersFromSyntax(syntax.Modifiers);
		modifiers |= DeclarationFacts.ModifierFromSyntaxKind(syntax.AccessibilityToken.Kind);

		return new(name, syntax, modifiers);
	}

	private FieldDeclaration AddField(FieldDeclarationSyntax syntax)
	{
		string name = syntax.Name.GetName();

		DeclarationModifiers modifiers = DeclarationFacts.ModifiersFromSyntax(syntax.Modifiers);
		modifiers |= DeclarationFacts.ModifierFromSyntaxKind(syntax.AccessibilityToken.Kind);

		return new(name, syntax, modifiers);
	}

	private TypeDeclaration AddType(TypeDeclarationSyntax syntax)
	{
		string name = syntax.Name.GetName();

		DeclarationModifiers modifiers = DeclarationFacts.ModifiersFromSyntax(syntax.Modifiers);
		modifiers |= DeclarationFacts.ModifierFromSyntaxKind(syntax.AccessibilityToken.Kind);

		return new(name, syntax, modifiers);
	}
}