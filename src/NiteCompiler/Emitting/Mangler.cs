using System.Diagnostics;
using System.Text;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.Emitting;

internal static class Mangler
{
	public static string Mangle(FunctionSymbol function)
	{
		Debug.Assert(!function.IsErrorSymbol);
		// TODO[generics]: monomorphized functions only

		StringBuilder sb = new();
		sb.Append(function.ToDisplayString(SymbolFormat.Metadata));
		// TODO[generics]: add mangled generics parameters
		return sb.ToString();
	}
}