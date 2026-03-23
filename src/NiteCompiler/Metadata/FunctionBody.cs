using System.Collections.Immutable;

namespace NiteCompiler.Metadata;

internal sealed class FunctionBody
{
	public readonly ImmutableArray<byte> IrBytes;

	public FunctionBody(ImmutableArray<byte> irBytes)
	{
		IrBytes = irBytes;
	}
}