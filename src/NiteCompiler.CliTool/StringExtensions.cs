namespace NiteCompiler.CliTool;

internal static class StringExtensions
{
	extension(string str)
	{
		public string TrimLineTerminators()
		{
			return str.ReplaceLineEndings(string.Empty);
		}
	}
}