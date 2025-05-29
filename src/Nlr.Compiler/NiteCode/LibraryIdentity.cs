using System;
using System.Security.AccessControl;

namespace Nlr.Compiler.NiteCode;

public sealed class LibraryIdentity : IEquatable<LibraryIdentity>
{
	private readonly string _name;

	private readonly SemVer _version;
	
	public string Name => _name;
	public SemVer Version => _version;

	private LibraryIdentity(LibraryIdentity copy, SemVer version)
	{
		_name = copy._name;
		_version = version;
	}


	public bool Equals(LibraryIdentity? other)
	{
		if (other is null) return false;
		if (ReferenceEquals(this, other)) return true;
		return _name == other._name && _version.Equals(other._version);
	}

	public override bool Equals(object? obj)
	{
		return ReferenceEquals(this, obj) || obj is LibraryIdentity other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(_name, _version);
	}
}