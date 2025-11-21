using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class TokenWithValue<T> : Token, ITokenWithValue
	where T : notnull
{
    public T Value { get; }
    object ITokenWithValue.Value => Value;
    public PredefinedType Type { get; }

    public TokenWithValue(SyntaxKind kind, TextSpan span, T value, PredefinedType type) : base(kind, span)
    {
	    Value = value;
	    Type = type;
    }

    public override string ToString()
    {
        return $"{Kind}<{typeof(T).Name}> := {Value} @{Span}";
    }
}