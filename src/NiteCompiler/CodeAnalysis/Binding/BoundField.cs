using System.Runtime.InteropServices;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundField : BoundNode
{
	public override BoundKind Kind => BoundKind.Field;
	public TypeSymbol Type { get; }
	public BoundStatement? Initializer { get; }

	public BoundField(SyntaxNode syntax, TypeSymbol type, [Optional] BoundStatement? initializer) : base(syntax)
	{
		Type = type;
		Initializer = initializer;
	}
}