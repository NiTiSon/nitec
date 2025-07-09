using System;
using System.Diagnostics;
using System.IO;
using Cysharp.IO;

namespace Nlr.Compiler.CodeAnalysis.Text;

public sealed class FileSource : Source
{
	private readonly FileInfo _file;
	private readonly Utf8StreamReader _streamReader;

	public override int Length => (int)_file.Length;

	public FileSource(FileInfo file)
	{
		long time = Stopwatch.GetTimestamp();
		_file = file;

		if (!_file.Exists)
		{
			throw new FileNotFoundException(null, fileName: _file.FullName);
		}
		Stream stream = _file.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
		_streamReader = new(stream, leaveOpen: false);

		Console.WriteLine(Stopwatch.GetElapsedTime(time));
	}

	public override void Dispose()
	{
		_streamReader.Dispose();
		base.Dispose();
	}

	public FileSource(string filePath) : this(new FileInfo(filePath)) { }


	public override void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
	{
	}

	public override string ToString()
	{
		return _file.FullName;
	}
}