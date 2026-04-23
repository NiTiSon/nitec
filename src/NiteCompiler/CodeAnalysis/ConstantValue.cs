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

	public virtual float F32 => throw new InvalidOperationException();
	public virtual double F64 => throw new InvalidOperationException();

	public static ConstantValue Create(bool value)
	{
		return value ? ValueBoolean.True : ValueBoolean.False;
	}

	public static ConstantValue Create(int value)
	{
		return new ValueI32(value);
	}

	private sealed class ValueI32 : ConstantValue
	{
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