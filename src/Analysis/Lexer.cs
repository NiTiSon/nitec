using System.Collections.Generic;

namespace NiteCompiler.Analysis;

public class Lexer
{
    private readonly string content;
    private int pos;
    public Lexer(string content)
    {
        this.content = content;
    }

    public IReadOnlyList<Token> GetTokens()
    {
        List<Token> tokens = new();

        Token? token;
    NEXT_TOKEN:
        token = NextToken();
        if (token is not null)
        {
            tokens.Add(token);
            goto NEXT_TOKEN;
        }

        return tokens;
    }
    public Token? NextToken()
    {
        if (content.Length <= pos)
            return null; // EOFToken!!
        
        pos++;
        return new TestToken();
    }
}