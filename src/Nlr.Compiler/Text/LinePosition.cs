using System;
using System.Numerics;

namespace Nlr.Compiler.Text;

/// <summary>
/// Representation of line and character position within source text.
/// </summary>
public readonly record struct LinePosition : IComparable<LinePosition>
{
	/// <summary>
	/// Zero-based line index.
	/// </summary>
	public uint Line { get; }

	/// <summary>
	/// Zero-based character index (column).
	/// </summary>
	public uint Column { get; }

	public LinePosition(uint line, uint column)
	{
		Line = line;
		Column = column;
	}

	/// <inheritdoc/>
	public readonly int CompareTo(LinePosition other)
	{
		int result = Line.CompareTo(other.Line);
		return (result != 0) ? result : Column.CompareTo(other.Column);
	}

	/// <summary>
	/// Return human-readable form of <see cref="LinePosition"/> instance.
	/// </summary>
	/// <returns>
	/// Human-readable representation of <see cref="LinePosition"/>.
	/// </returns>
	public override string ToString()
	{
		return $"{Line + 1}:{Column + 1}";
	}

	/// <inheritdoc/>
	public static bool operator <(LinePosition left, LinePosition right)
	{
		return left.CompareTo(right) < 0;
	}

	/// <inheritdoc/>
	public static bool operator <=(LinePosition left, LinePosition right)
	{
		return left.CompareTo(right) <= 0;
	}

	/// <inheritdoc/>
	public static bool operator >(LinePosition left, LinePosition right)
	{
		return left.CompareTo(right) > 0;
	}

	/// <inheritdoc/>
	public static bool operator >=(LinePosition left, LinePosition right)
	{
		return left.CompareTo(right) >= 0;
	}
}