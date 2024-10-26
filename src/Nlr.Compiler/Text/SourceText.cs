using Microsoft.Extensions.Primitives;
using System;

namespace Nlr.Compiler.Text;

// TODO: Better implementation!
public sealed class SourceText
{
	private readonly string text;

	public SourceText(string text)
	{
		this.text = text;
	}

	public uint Length => (uint)text.Length;

	internal char this[uint index]
	{
		get
		{
			return text[(int)index];
		}
	}

	public StringSegment Substring(uint position, uint length)
	{
		return new StringSegment(this.text, (int)position, (int)length);
	}
}