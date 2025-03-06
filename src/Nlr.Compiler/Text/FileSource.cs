using System;
using System.Buffers;
using System.IO;
using System.Runtime.InteropServices;

namespace Nlr.Compiler.Text;

public sealed class FileSource : Source
{
	private readonly FileInfo _file;
	private readonly FileStream _fs;

	public FileSource(Compilation compilation, FileInfo file) : base(compilation)
	{
		_file = file;
		_fs = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
	}

	public override int Length => (int)_file.Length;

	public override void CopyTo(int position, Utf8Char[] destination, int offset, int count)
	{
		_fs.Seek(position, SeekOrigin.Begin);
		_fs.ReadExactly(MemoryMarshal.Cast<Utf8Char, byte>(destination.AsSpan().Slice(offset, count)));
	}
}
