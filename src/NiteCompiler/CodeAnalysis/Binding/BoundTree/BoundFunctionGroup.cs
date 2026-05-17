using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Binding.Pure;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundFunctionGroup : BoundExpression
{
	public ImmutableArray<FunctionSymbol> Candidates { get; }
	public BoundExpression? Receiver { get; }
	public LookupResultKind ResultKind { get; }

	public BoundFunctionGroup(SyntaxNode syntax, ImmutableArray<FunctionSymbol> candidates,
		BoundExpression? receiver, LookupResultKind resultKind, TypeSymbol type, bool hasErrors = false)
		: base(syntax, hasErrors)
	{
		Candidates = candidates;
		Receiver = receiver;
		ResultKind = resultKind;
		Type = type;
	}

	public override BoundKind Kind => BoundKind.FunctionGroup;
	public override TypeSymbol Type { get; }
	public override Pureness Pureness => Pureness.None;
	public override Binder.BindValueKind ValueKind => Binder.BindValueKind.RValueOrFunctionGroup;

	public override void Accept(BoundVisitor visitor) => visitor.VisitMethodGroup(this);
}
