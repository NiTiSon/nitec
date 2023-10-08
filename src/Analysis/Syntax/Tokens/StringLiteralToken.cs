using Microsoft.Extensions.Primitives;
using NiteCompiler.Analysis.Text;

namespace NiteCompiler.Analysis.Syntax.Tokens;

public sealed class StringLiteralToken : LiteralToken
{
	public readonly StringLiteralKind LiteralKind;
	public StringLiteralToken(StringLiteralKind kind, TextAnchor location, StringSegment content) : base(SyntaxKind.StringLiteral, location, content)
	{
		LiteralKind = kind;
	}

	public override string ToString()
		=> $"{GetStringNameDebug()} {Content} @{Location}";

	private string GetStringNameDebug()
	{
		return LiteralKind switch
		{
			StringLiteralKind.U8 => "UTF-8 string",
			StringLiteralKind.U16 => "UTF-16 string",
			StringLiteralKind.U32 => "UTF-32 string",
			_ => "string"
		};
	}
}
public sealed class BooleanLiteralToken : LiteralToken
{
	public readonly bool Value;
	public BooleanLiteralToken(bool value, TextAnchor location, StringSegment content) : base(SyntaxKind.BooleanLiteral, location, content)
	{
		Value = value;
	}
}
