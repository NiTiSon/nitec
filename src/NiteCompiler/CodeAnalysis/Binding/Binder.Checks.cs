using System;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding;

internal partial class Binder
{
	private const int ValueKindInsignificantBits = 2;
	private const BindValueKind ValueKindSignificantBitsMask = unchecked((BindValueKind)~((1 << ValueKindInsignificantBits) - 1));

	[Flags]
	internal enum BindValueKind : ushort
	{
		RValue = 1 << ValueKindInsignificantBits,

		LValue = 2 << ValueKindInsignificantBits,

		RefersToLocation = 4 << ValueKindInsignificantBits,
	}

	private BoundExpression CheckValue(BoundExpression expression, BindValueKind valueKind, BindingDiagnosticBag diagnostics)
	{
		BindValueKind actual = expression.ValueKind;

		if ((actual & ValueKindSignificantBitsMask) == (valueKind & ValueKindSignificantBitsMask))
			return expression;

		// TODO: Error in diagnostic

		return expression;
	}
}