namespace NiteCode.Compiler.CodeAnalysis;

public static class WellKnownMemberNames
{
	public const string AdditionOperatorName = "op_addition";
	public const string SubtractionOperatorName = "op_subtraction";
	public const string DivisionOperatorName = "op_division";
	public const string MultiplicationOperatorName = "op_multiplication";
	public const string ModulusOperatorName = "op_modulus";
	public const string BitwiseAndOperatorName = "op_and";
	public const string BitwiseOrOperatorName = "op_or";
	public const string BitwiseExclusiveOrOperatorName = "op_xor";

	public const string IncrementOperatorName = "op_increment";
	public const string DecrementOperatorName = "op_decrement";

	public const string GlobalModule = "<global>";

	public const string EntryPointName = "main";
	public const string ConstructorName = "op_new";
	public const string TypeConstructorName = "type_init";
	public const string EnumBackingFieldName = "value__";

	public const string DisposeMethodName = "dispose";
}