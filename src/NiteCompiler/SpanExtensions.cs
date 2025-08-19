using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;

namespace NiteCompiler;

public static class SpanExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe bool Contains<TEnum>(this ReadOnlySpan<TEnum> span, TEnum value)
		where TEnum : unmanaged, Enum
	{
		switch (sizeof(TEnum))
		{
			case sizeof(byte):
				return MemoryMarshal.Cast<TEnum, byte>(span).Contains(Unsafe.BitCast<TEnum, byte>(value));
			case sizeof(ushort):
				return MemoryMarshal.Cast<TEnum, ushort>(span).Contains(Unsafe.BitCast<TEnum, ushort>(value));
			case sizeof(uint):
				return MemoryMarshal.Cast<TEnum, uint>(span).Contains(Unsafe.BitCast<TEnum, uint>(value));
			case sizeof(ulong):
				return MemoryMarshal.Cast<TEnum, ulong>(span).Contains(Unsafe.BitCast<TEnum, ulong>(value));
			default:
				ThrowHelper.ThrowNotSupportedException();
				return false;
		}
	}
}