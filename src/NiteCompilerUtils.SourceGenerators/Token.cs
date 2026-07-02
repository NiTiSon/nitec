namespace NiteCompilerUtils.SourceGenerators;

public record struct Token
{
	public string Name;
	public string? Text;
	public string? Binary;
	public string? Unary;
}