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
		BindValueKind actual = expression.ValueKind & ValueKindSignificantBitsMask;
		BindValueKind expected = valueKind & ValueKindSignificantBitsMask;

		if ((actual & expected) != 0)
			return expression;

		if (expression.HasErrors || expression.Type.IsErrorSymbol)
		{
			return expression;
		}

		if ((expected & BindValueKind.LValue) != 0)
		{
			diagnostics.Diagnostics.ReportCannotUseAsLValue(expression.Syntax!.Location);
		}
		else if ((expected & BindValueKind.RValue) != 0)
		{
			diagnostics.Diagnostics.ReportCannotUseAsRValue(expression.Syntax!.Location);
		}

		return BadExpression(expression.Syntax!);
	}
}
