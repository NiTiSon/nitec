using NiteCompiler.Analysis.Syntax.Tokens;

namespace NiteCompiler.Analysis;

public interface TokenStream
{
	public bool IsEOF { get; }
	public Token NextToken();
}
