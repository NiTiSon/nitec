using System;
using System.IO;
using System.Linq;
using System.Text;

namespace NiteLang.Metadata;

public sealed class NlibWriter : IDisposable
{
	internal const ushort LastWriterVersion = 1;

	private readonly Stream _stream;
	private readonly BinaryWriter _writer;
	private readonly bool _leaveOpen;

	public NlibWriter(Stream stream, bool leaveOpen)
	{
		_stream = stream;
		_writer = new(_stream, Encoding.UTF8, true);
		_leaveOpen = leaveOpen;
	}

	public void Dispose()
	{
		if (!_leaveOpen)
		{
			_stream.Dispose();
		}
		_writer.Dispose();
	}

	public void Write(NlibLibraryBuilder builder)
	{
		NlibModuleBuilder[] modules = builder.Modules.ToArray();
		NlibTypeBuilder[] types = builder.Types.ToArray();

		Table<StringConstant> strings = new();
		Handle libName = strings.Add(builder.Name);

		// 0x00
		_writer.Write(NlibConstants.MagicNumber);
		_writer.Write(LastWriterVersion);
		_writer.Write((ushort)0);
		_writer.Write(libName);
		_writer.Write(0);
		// 0x10
		_writer.Write((ulong)0); // MAIN FUNCTION
		_writer.Write((ulong)0);

		WriteStringTable(strings);
	}

	private void WriteStringTable(Table<StringConstant> strings)
	{
		long headerPosition = _writer.BaseStream.Position;
		_writer.Seek(16, SeekOrigin.Current);
		foreach (StringConstant constant in strings)
		{
			_writer.Write(constant.Value);
		}
		long size = _writer.BaseStream.Position - headerPosition;
		_writer.Seek((int)headerPosition, SeekOrigin.Begin);
		WriteTableHeader(StringConstant.TableStorageType, (uint)strings.Count, (uint)size);
		_writer.Seek((int)(headerPosition + size), SeekOrigin.Begin);
	}

	private void WriteTableHeader(TableType table, uint count, uint tableSizeWithHeader)
	{
		_writer.Write((byte)table);
		_writer.Write(stackalloc byte[7]);
		_writer.Write(count);
		_writer.Write(tableSizeWithHeader);
	}
}