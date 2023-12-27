using Microsoft.Extensions.Primitives;

namespace NiteCode.Compiler.CodeAnalysis.Text;

public readonly struct SourceText(string text, string? filename)
{
	public readonly string Text = text;
	public readonly string? Filename = filename;

	public readonly uint Length => (uint)Text.Length;

	public StringSegment ToString(uint from, uint length)
		=> new(Text, (int)from, (int)length);
}