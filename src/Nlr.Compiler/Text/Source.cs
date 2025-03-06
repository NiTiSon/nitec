using System;

namespace Nlr.Compiler.Text;

public abstract class Source
{
	protected readonly Compilation compilation;

	protected Source(Compilation compilation)
	{
		this.compilation = compilation;
	}

	public abstract int Length { get; }

	public abstract void CopyTo(int position, Utf8Char[] destination, int offset, int count);
}