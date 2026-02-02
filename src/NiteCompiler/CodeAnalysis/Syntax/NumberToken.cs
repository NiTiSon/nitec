using System.Runtime.InteropServices;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class NumberToken : Token
{
	public Packed Value { get; }
	public NumericLiteralType Type { get; }
	public NumericLiteralFormat Format { get; }
	public override TokenKind TKind => TokenKind.NumberLiteral;

	public NumberToken(SyntaxTree tree, TextSpan span, Packed value, NumericLiteralType type, NumericLiteralFormat format,
		SyntaxList<Trivia> leadingTrivia, SyntaxList<Trivia> trailingTrivia) : base(tree, span, leadingTrivia, trailingTrivia)
	{
		Value = value;
		Type = type;
		Format = format;
	}

	[StructLayout(LayoutKind.Explicit)]
	public readonly struct Packed
	{
		[FieldOffset(0)] public readonly ulong U64;
		[FieldOffset(0)] public readonly double F64;

		public Packed(ulong value)
		{
			U64 = value;
		}

		public Packed(double value)
		{
			F64 = value;
		}
	}

	public override string ToString()
	{
		return base.ToString() + $"int := {Value.U64} | real := {Value.F64}";
	}
}