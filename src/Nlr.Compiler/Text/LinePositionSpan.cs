using System;

namespace Nlr.Compiler.Text;

public readonly record struct LinePositionSpan
{
	public LinePosition Begin { get; }
	
	public LinePosition End { get; }

	public bool IsMultiline => Begin.Line != End.Line;
	
	public LinePositionSpan(LinePosition begin, LinePosition end)
	{
		ArgumentOutOfRangeException.ThrowIfGreaterThan(end, begin);

		Begin = begin;
		End = end;
	}

}