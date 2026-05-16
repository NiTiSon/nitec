using NiteCompiler.CodeAnalysis.Binding.Pure;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundFieldAccess : BoundExpression
{
	public BoundExpression Receiver { get; }
	public FieldSymbol Field { get; }

	public override BoundKind Kind => BoundKind.FieldAccess;
	public override TypeSymbol Type => Field.Type;
	public override Pureness Pureness
	{
		get
		{
			Pureness pureness = Pureness.Pure;
			pureness += Receiver.Pureness;
			return pureness;
		}
	}
	public override Binder.BindValueKind ValueKind => Binder.BindValueKind.LValue | Binder.BindValueKind.RValue | Binder.BindValueKind.RefersToLocation;

	public BoundFieldAccess(SyntaxNode syntax, BoundExpression receiver, FieldSymbol field, bool hasErrors = false)
		: base(syntax, hasErrors || receiver.HasErrors)
	{
		Receiver = receiver;
		Field = field;
	}

	public override void Accept(BoundVisitor visitor) => visitor.VisitFieldAccess(this);
}
