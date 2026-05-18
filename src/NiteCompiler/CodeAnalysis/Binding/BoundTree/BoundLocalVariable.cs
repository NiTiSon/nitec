using NiteCompiler.CodeAnalysis.Binding.Pure;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundLocalVariable : BoundExpression
{
	public LocalVariableSymbol Variable { get; }

	public BoundLocalVariable(SyntaxNode syntax, LocalVariableSymbol variable) : base(syntax)
	{
		Variable = variable;
	}

	public override BoundKind Kind => BoundKind.LocalVariable;
	public override TypeSymbol Type => Variable.Type;
	public override Pureness Pureness => Pureness.None;
	public override Binder.BindValueKind ValueKind => Binder.BindValueKind.LValue | Binder.BindValueKind.RValue | Binder.BindValueKind.RefersToLocation;

	public override void Accept(BoundVisitor visitor) => visitor.VisitLocalVariable(this);
}
