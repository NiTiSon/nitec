using Microsoft.Extensions.Primitives;
using System;
using System.IO;

namespace Nlr.Compiler.Text;

// TODO: Better implementation!
public sealed class SourceText
{
	private readonly FileInfo file;
	private readonly string text;

	public string Path => file.FullName;

	public string Content => text;

	public SourceText(FileInfo file)
	{
		ArgumentNullException.ThrowIfNull(file);
		
		this.file = file;
		this.text = File.ReadAllText(file.FullName);
	}

	public SourceText(string text) : this(text, "script") {}
	
	public SourceText(string text, string path)
	{
		this.text = text;
		this.file = new FileInfo(path);
	}

	public uint Length => (uint)text.Length;

	internal char this[uint index]
	{
		get
		{
			return text[(int)index];
		}
	}

	public StringSegment Substring(TextSpan span)
	{
		return Substring(span.Begin, span.Length);
	}
	
	public StringSegment Substring(uint position, uint length)
	{
		return new StringSegment(this.text, (int)position, (int)length);
	}
}