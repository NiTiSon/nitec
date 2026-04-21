using System;
using System.Threading;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceLocalVariableSymbol : LocalVariableSymbol
{
	private Binder _scopeBinder;
	private TypeClause? _typeClause;
	private EqualsValueClause? _equalsValueClause;
	private Binder? _initializerBinder;

	public bool IsAssignable { get; }
	public override string Name { get; }

	public override TypeSymbol Type
	{
		get
		{
			// if (field == null)
			// {
			// 	Interlocked.CompareExchange(ref field, InferType(), null);
			// }
			//
			// return field;
			throw new NotImplementedException();
		}
	}

	public override Symbol ContainingSymbol { get; }

	public SourceLocalVariableSymbol(Symbol containing, Binder scopeBinder,
		TypeClause? type, EqualsValueClause? initializer, Binder? initializerBinder,
		bool isAssignable, string name,
		Location nameLocation, SyntaxReference syntaxReference)
	{
		ContainingSymbol = containing;

		IsAssignable = isAssignable;
		Name = name;
	}
}