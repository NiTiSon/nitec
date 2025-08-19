using System.Collections.Generic;
using System.IO;

namespace NiTiS.Compiler.CliTool;

internal sealed class FileInfoFullNameComparer : IEqualityComparer<FileInfo>
{
	private FileInfoFullNameComparer() { }
	public static FileInfoFullNameComparer Instance { get; } = new();

	public bool Equals(FileInfo? x, FileInfo? y)
	{
		if (ReferenceEquals(x, y)) return true;
		if (x is null) return false;
		if (y is null) return false;
		if (x.GetType() != y.GetType()) return false;
		return x.FullName == y.FullName;
	}

	public int GetHashCode(FileInfo obj)
	{
		return obj.FullName.GetHashCode();
	}
}