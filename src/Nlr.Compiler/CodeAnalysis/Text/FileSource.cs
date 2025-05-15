using System.IO;

namespace Nlr.Compiler.CodeAnalysis.Text;

public sealed class FileSource : Source
{
	private readonly FileInfo _file;
	private readonly string _content;

	public override int Length => (int)_file.Length;

	public FileSource(FileInfo file)
	{
		_file = file;

		if (!_file.Exists)
		{
			throw new FileNotFoundException(null, fileName: _file.FullName);
		}

		_content = File.ReadAllText(_file.FullName);
	}

	public FileSource(string filePath) : this(new FileInfo(filePath)) { }


	public override void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
	{
		_content.CopyTo(sourceIndex, destination, destinationIndex, count);
	}

	public override string ToString()
	{
		return _file.FullName;
	}
}