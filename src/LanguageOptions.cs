namespace NiteCompiler;

public sealed class LanguageOptions
{
	public required int Version { get; init; }

	public static readonly LanguageOptions StandardV1 = new()
	{
		Version = 1,
	};
}