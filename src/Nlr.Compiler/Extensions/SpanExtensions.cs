using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;

namespace Nlr.Compiler.Extensions;

public static class SpanExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe bool Contains<TEnum>(this ReadOnlySpan<TEnum> span, TEnum value)
		where TEnum : unmanaged, Enum
	{
		if (sizeof(byte) == sizeof(TEnum))
		{
			return MemoryMarshal.Cast<TEnum, byte>(span).Contains(Unsafe.BitCast<TEnum, byte>(value));
		}
		else if (sizeof(ushort) == sizeof(TEnum))
		{
			return MemoryMarshal.Cast<TEnum, ushort>(span).Contains(Unsafe.BitCast<TEnum, ushort>(value));
		}
		else if (sizeof(uint) == sizeof(TEnum))
		{
			return MemoryMarshal.Cast<TEnum, uint>(span).Contains(Unsafe.BitCast<TEnum, uint>(value));
		}
		else if (sizeof(ulong) == sizeof(TEnum))
		{
			return MemoryMarshal.Cast<TEnum, ulong>(span).Contains(Unsafe.BitCast<TEnum, ulong>(value));
		}
		else
		{
			ThrowHelper.ThrowNotSupportedException();
			return false;
		}
	}
}