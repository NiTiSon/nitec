using System;

namespace NiteCompiler.CodeAnalysis;

internal abstract class ConstantValue
{
	public abstract SpecialType SpecialType { get; }

	public virtual bool Bool => throw new InvalidOperationException();
	public virtual sbyte S8 => throw new InvalidOperationException();
	public virtual byte U8 => throw new InvalidOperationException();
	public virtual short S16 => S8;
	public virtual ushort U16 => U8;
	public virtual int S32 => S16;
	public virtual uint U32 => U16;
	public virtual long S64 => S32;
	public virtual ulong U64 => U32;

	public virtual Half F16 => throw new InvalidOperationException();
	public virtual float F32 => (float)F16;
	public virtual double F64 => (double)F32;

	public static ConstantValue Create(bool value)
	{
		return value ? ValueBoolean.True : ValueBoolean.False;
	}

	public static ConstantValue Create(int value)
	{
		return value switch
		{
			0 => ValueI32.Zero,
			1 => ValueI32.One,
			-1 => ValueI32.MinusOne,
			2 => ValueI32.Two,
			_ => new ValueI32(value)
		};
	}

	public static ConstantValue Create(long value)
	{
		return value switch
		{
			0 => ValueI64.Zero,
			1 => ValueI64.One,
			-1 => ValueI64.MinusOne,
			2 => ValueI64.Two,
			_ => new ValueI64(value)
		};
	}

	public static ConstantValue Create(Half value)
	{
		return new ValueF16(value);
	}

	public static ConstantValue Create(float value)
	{
		return new ValueF32(value);
	}

	public static ConstantValue Create(double value)
	{
		return new ValueF64(value);
	}

	private sealed class ValueI32 : ConstantValue
	{
		public static readonly ValueI32 Zero = new(0);
		public static readonly ValueI32 One = new(1);
		public static readonly ValueI32 MinusOne = new(-1);
		public static readonly ValueI32 Two = new(2);
		private readonly uint _value;

		public ValueI32(int value)
		{
			this._value = unchecked((uint)value);
		}

		public ValueI32(uint value)
		{
			this._value = value;
		}

		public override SpecialType SpecialType => SpecialType.StdNumericsSInt32;

		public override uint U32 => _value;
		public override int S32 => unchecked((int)_value);
	}

	private sealed class ValueI64 : ConstantValue
	{
		public static readonly ValueI64 Zero = new(0);
		public static readonly ValueI64 One = new(1);
		public static readonly ValueI64 MinusOne = new(-1);
		public static readonly ValueI64 Two = new(2);
		private readonly ulong _value;

		public ValueI64(long value)
		{
			this._value = unchecked((uint)value);
		}

		public ValueI64(ulong value)
		{
			this._value = value;
		}

		public override SpecialType SpecialType => SpecialType.StdNumericsSInt64;

		public override ulong U64 => _value;
		public override int S32 => unchecked((int)_value);
	}

	private sealed class ValueF16 : ConstantValue
	{
		private readonly Half _value;

		public ValueF16(Half value)
		{
			_value = value;
		}

		public override SpecialType SpecialType => SpecialType.StdNumericsFloat16;

		public override Half F16 => _value;
	}

	private sealed class ValueF32 : ConstantValue
	{
		private readonly float _value;

		public ValueF32(float value)
		{
			_value = value;
		}

		public override SpecialType SpecialType => SpecialType.StdNumericsFloat32;

		public override float F32 => _value;
	}

	private sealed class ValueF64 : ConstantValue
	{
		private readonly double _value;

		public ValueF64(double value)
		{
			_value = value;
		}

		public override SpecialType SpecialType => SpecialType.StdNumericsFloat64;

		public override double F64 => _value;
	}

	private sealed class ValueBoolean : ConstantValue
	{
		public static readonly ValueBoolean True = new(true);
		public static readonly ValueBoolean False = new(false);
		private readonly bool _value;

		private ValueBoolean(bool value) { _value = value; }

		public override SpecialType SpecialType => SpecialType.StdBoolean;
		public override bool Bool => _value;
	}
}