using NiteCompiler.CodeAnalysis.Binding.Pure;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundParameter : BoundExpression
{
	public ParameterSymbol Parameter { get; }

	public BoundParameter(SyntaxNode syntax, ParameterSymbol parameter) : base(syntax)
	{
		Parameter = parameter;
	}

	public override BoundKind Kind => BoundKind.Parameter;
	public override TypeSymbol Type => Parameter.Type;
	public override Pureness Pureness => Pureness.None; // TODO: pure attribute on parameters? or get pureness from owning function symbol
	public override Binder.BindValueKind ValueKind => Binder.BindValueKind.LValue |
	                                                  Binder.BindValueKind.RValue |
	                                                  Binder.BindValueKind.RefersToLocation;

	public override void Accept(BoundVisitor visitor)
	{
		visitor.VisitParameter(this);
	}
}