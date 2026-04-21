using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundLocalVariableDeclarationStatement : BoundStatement
{
	public LocalVariableSymbol Local { get; }
	public BoundExpression? Initializer { get; }

	public BoundLocalVariableDeclarationStatement(LocalVariableDeclarator syntax, LocalVariableSymbol local,
		BoundExpression? initializer, bool hasErrors = false) : base(syntax, hasErrors)
	{
		Local = local;
		Initializer = initializer;
	}

	public override BoundKind Kind => BoundKind.VariableDeclarationStatement;
	public override void Accept(BoundVisitor visitor)
	{
		visitor.VisitLocalVariableDeclarationStatement(this);
	}
}