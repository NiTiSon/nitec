using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed partial class NiteLexer
{
    private readonly SlidingWindow _window;

    public NiteLexer(SourceText source)
    {
        _window = new SlidingWindow(source);
    }

    internal ref struct TokenInfo
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
        string? text = info.Kind
            is SyntaxKind.IdentifierToken
            or SyntaxKind.NumberToken
                ? _window.Lexeme
                : null;
        
        ReadTrivia(leading: false);

        switch (info.Kind)
        {
            case SyntaxKind.IdentifierToken:
                return new IdentifierToken(info.Kind, info.ContextualKind, _window.LexemeSpan, text!);
            case SyntaxKind.NumberToken:
                return null!;
            default:
                return new Token(info.Kind, span);
        }
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
                _window.Advance();
                ReadIdentifierSkipFirst(ref info);
                break;
            case ':':
                _window.Advance();
                if (_window.Current == ':')
                {
                    info.Kind = SyntaxKind.ColonColonToken;
                    _window.Advance();
                }
                else
                {
                    info.Kind = SyntaxKind.ColonToken;
                }
                break;
            case '-':
                _window.Advance();
                if (_window.Current == '>')
                {
                    _window.Advance();
                    info.Kind = SyntaxKind.RetusaToken;
                }
                else
                {
                    info.Kind = SyntaxKind.MinusToken;
                }
                break;
            case '=':
                _window.Advance();
                info.Kind = SyntaxKind.EqualsToken;
                break;
            case ';':
                _window.Advance();
                info.Kind = SyntaxKind.SemicolonToken;
                break;
            case '{':
                _window.Advance();
                info.Kind = SyntaxKind.OpenBraceToken;
                break;
            case '}':
                _window.Advance();
                info.Kind = SyntaxKind.CloseBraceToken;
                break;
            case '(':
                _window.Advance();
                info.Kind = SyntaxKind.OpenParenToken;
                break;
            case ')':
                _window.Advance();
                info.Kind = SyntaxKind.CloseParenToken;
                break;
            case >= '0' and <= '9':
                info.Kind = SyntaxKind.NumberToken;
                _window.Advance();
                while (char.IsAsciiDigit(_window.Current))
                {
                    _window.Advance();
                }
                break;
            default:
                ReadIdentifier(ref info);

                if (_window.Width == 0)
                {
                    _window.Advance();
                    info.Kind = SyntaxKind.Invalid;
                }
                break;
        }

        if (info.Kind == SyntaxKind.IdentifierToken && SyntaxFacts.IsPossibleKeyword(_window.Width))
        {
            SyntaxFacts.DefineKeywordOrIdentifier(_window.Lexeme, ref info);
        }
    }
}