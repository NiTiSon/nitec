using System.IO;

namespace Nlr.CodeGen;

public sealed class LibraryBuilder : Library
{
	public override string Name { get; }

	public LibraryBuilder(string libraryName)
	{
		Name = libraryName;
	}

	public object CreateDefaultMode()
	{
		throw null!;
	}

	public object CreateModule(string name)
	{
		throw null!;
	}

	public void Save(BinaryWriter writer)
	{
		writer.Write((byte)0x73);
		writer.Write((byte)'N');
		writer.Write((byte)'D');
		writer.Write((byte)'L');
	}
}