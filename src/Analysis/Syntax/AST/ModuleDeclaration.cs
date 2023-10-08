using NiteCompiler.Analysis.Syntax.Tokens;

namespace NiteCompiler.Analysis.Syntax.AST;

public sealed class ModuleDeclaration : SyntaxTree, Declaration
{
	public readonly IdentifierOrKeywordToken ModuleKw;
	public readonly Identifier ModuleName;
	public ModuleDeclaration(SyntaxKind kind, IdentifierOrKeywordToken moduleKw, Identifier moduleName) : base(kind)
	{
		ModuleKw = moduleKw;
		ModuleName = moduleName;
	}
}
