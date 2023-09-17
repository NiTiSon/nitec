using NiteCompiler.Analysis.Text;

namespace NiteCompiler.Analysis.Tokens;

public sealed class OperatorToken : Token
{
	public OperatorToken(TextAnchor position, SyntaxKind operatorKind)
		: base(operatorKind, position)
	{ }

    public override string ToString()
		=> $"Op: '{Kind}' @{Location}";
}