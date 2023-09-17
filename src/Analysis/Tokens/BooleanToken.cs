using NiteCompiler.Analysis.Text;

namespace NiteCompiler.Analysis.Tokens;

public sealed class BooleanToken : Token
{
    public readonly bool Value;
	public BooleanToken(TextAnchor position, bool value)
		: base(SyntaxKind.BooleanLiteral, position)
	{
        Value = value;
	}

	public override string ToString()
		=> $"Literal<Bool>: '{Value}' @{Location}";
}