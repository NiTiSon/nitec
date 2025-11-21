namespace NiteCompiler.CodeAnalysis.Syntax;

internal interface ITokenWithValue
{
	object Value { get; }
	PredefinedType Type { get; }
}