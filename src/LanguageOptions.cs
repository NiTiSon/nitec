namespace NiteCompiler;

public sealed class LanguageOptions
{
	public required uint Version { get; init; }

	public static readonly LanguageOptions StandardV1 = new()
	{
		Version = 1,
	};
}