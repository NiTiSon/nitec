using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
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
	public override Binder.BindValueKind ValueKind => Binder.BindValueKind.LValue | Binder.BindValueKind.RValue;

	public override void Accept(BoundVisitor visitor)
	{
		visitor.VisitParameter(this);
	}
}