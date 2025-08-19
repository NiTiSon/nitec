using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed partial class NiteLexer
{
    private readonly SlidingWindow _window;

    public NiteLexer(SourceText source)
    {
        _window = new SlidingWindow(source);
    }

    private ref struct TokenInfo
    {
        public SyntaxKind Kind;
        public SyntaxKind ContextualKind;
    }

    public Token Lex()
    {
        TokenInfo info = default;

        ReadTrivia(leading: true);
        
        _window.Start();
        ReadToken(ref info);
        TextSpan span = _window.LexemeSpan;
        
        ReadTrivia(leading: false);

        return new Token(info.Kind, span);
    }

    private void ReadToken(ref TokenInfo info)
    {
        if (_window.IsAtTheEnd)
        {
            info.Kind = SyntaxKind.EndOfFile;
            return;
        }

        switch (_window.Current)
        {
            case >= 'a' and <= 'z':
            case >= 'A' and <= 'Z':
                ReadIdentifier(ref info);
                return;
        }
        
        info.Kind = SyntaxKind.Invalid;
        _window.Advance();
    }
}