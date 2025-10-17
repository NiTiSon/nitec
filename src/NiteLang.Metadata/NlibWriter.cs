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
		NlibModuleBuilder[] defModules = builder.Modules.ToArray();
		NlibTypeBuilder[] defTypes = builder.Types.ToArray();

		Table<StringConstant> strings = [];
		Handle libName = strings.Add(builder.Name);

		Table<NlibModuleBuilder> modules = [..defModules];
		Table<NlibTypeBuilder> types = [..defTypes];

		// 0x00
		_writer.Write(NlibConstants.MagicNumber);
		_writer.Write(LastWriterVersion);
		_writer.Write((ushort)0);
		_writer.Write(libName);
		_writer.Write(0);
		// 0x10
		_writer.Write((ulong)0); // MAIN FUNCTION
		_writer.Write((ulong)0);

		WriteModuleTable(modules, strings);
		Align();
		WriteTypeTable(types, modules, strings);
		Align();
		WriteStringTable(strings);
		Align();
	}

	private void Align()
	{
		long needToAlign = _stream.Position % 16;
		if (needToAlign != 0)
		{
			// Why there's no method to repetitive Write?
			_writer.Write(stackalloc byte[16 - (int)needToAlign]);
		}
	}

	private void WriteTypeTable(Table<NlibTypeBuilder> types, Table<NlibModuleBuilder> modules, Table<StringConstant> strings)
	{
		long headerPosition = _writer.BaseStream.Position;
		_writer.Seek(16, SeekOrigin.Current);
		foreach (NlibTypeBuilder type in types)
		{
			type.Write(_writer, modules, strings);
		}
		long size = _writer.BaseStream.Position - headerPosition;
		_writer.Seek((int)headerPosition, SeekOrigin.Begin);
		WriteTableHeader(NlibTypeBuilder.TableStorageType, (uint)strings.Count, (uint)size);
		_writer.Seek((int)(headerPosition + size), SeekOrigin.Begin);
	}

	private void WriteStringTable(Table<StringConstant> strings)
	{
		long headerPosition = _writer.BaseStream.Position;
		_writer.Seek(16, SeekOrigin.Current);
		foreach (StringConstant str in strings)
		{
			_writer.Write(str.Value);
		}
		long size = _writer.BaseStream.Position - headerPosition;
		_writer.Seek((int)headerPosition, SeekOrigin.Begin);
		WriteTableHeader(StringConstant.TableStorageType, (uint)strings.Count, (uint)size);
		_writer.Seek((int)(headerPosition + size), SeekOrigin.Begin);
	}

	private void WriteModuleTable(Table<NlibModuleBuilder> modules, Table<StringConstant> strings)
	{
		long headerPosition = _writer.BaseStream.Position;
		_writer.Seek(16, SeekOrigin.Current);
		foreach (NlibModuleBuilder module in modules)
		{
			module.Write(_writer, strings);
		}
		long size = _writer.BaseStream.Position - headerPosition;
		_writer.Seek((int)headerPosition, SeekOrigin.Begin);
		WriteTableHeader(NlibModuleBuilder.TableStorageType, (uint)modules.Count, (uint)size);
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