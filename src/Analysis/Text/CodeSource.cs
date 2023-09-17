namespace NiteCompiler.Analysis.Text;

public sealed class CodeSource
{
	public readonly string? File;
	public readonly string Content;

	public CodeSource(string content, string? file = null)
	{
		Content = content;
		File = file;
	}

	public override string ToString()
		=> $"{{{(File is not null ? File : "unknown file")}}}";
}