using NiteCompiler.CodeAnalysis.Binding.Pure;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundLocal : BoundExpression
{
	public LocalVariableSymbol Local { get; }

	public BoundLocal(SyntaxNode syntax, LocalVariableSymbol local) : base(syntax)
	{
		Local = local;
	}

	public override BoundKind Kind => BoundKind.Local;
	public override TypeSymbol Type => Local.Type;
	public override Pureness Pureness => Pureness.None; // TODO: pure locals?
	public override Binder.BindValueKind ValueKind => Binder.BindValueKind.LValue;

	public override void Accept(BoundVisitor visitor)
	{
		visitor.VisitLocal(this);
	}
}