namespace Nlr.Compiler.CodeAnalysis.Text;

public sealed class TextSource : Source
{
	private readonly string _text;

	public TextSource(string text)
	{
		_text = text;
	}

	public override void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
	{
		_text.CopyTo(sourceIndex, destination, destinationIndex, count);
	}

	public override int Length => _text.Length;
}