using NiteCompiler.CodeAnalysis.Binding.Pure;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundMove : BoundExpression
{
	public LocalVariableOrParameterSymbol Variable { get; }

	public BoundMove(SyntaxNode syntax, LocalVariableOrParameterSymbol variable) : base(syntax)
	{
		Variable = variable;
	}

	public override BoundKind Kind => BoundKind.Move;
	public override TypeSymbol Type => Variable.Type;
	public override Pureness Pureness => Pureness.None;
	public override Binder.BindValueKind ValueKind => Binder.BindValueKind.LValue |
	                                                  Binder.BindValueKind.RValue |
	                                                  Binder.BindValueKind.RefersToLocation;

	public override void Accept(BoundVisitor visitor) => visitor.VisitMove(this);
}
