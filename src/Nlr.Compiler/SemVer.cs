using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Primitives;

namespace Nlr.Compiler;

public readonly struct SemVer :
	IEquatable<SemVer>,
	IComparable<SemVer>,
	IComparisonOperators<SemVer, SemVer, bool>,
	ISpanParsable<SemVer>
{
	private readonly uint _major, _minor, _patch;
	private readonly string? _tail;
	private readonly int _prereleasePosition, _metaPosition;
	
	public uint Major => _major;
	public uint Minor => _minor;
	public uint Patch => _patch;
	
	public bool IsContainsPrerelease
		=> _prereleasePosition >= 0;
	
	public bool IsContainsMeta
		=> _metaPosition >= 0;

	public IList<string> PrereleaseIdentifiers
	{
		get
		{
			if (!IsContainsPrerelease) return Array.Empty<string>();
        
			ReadOnlySpan<char> identifiersSpan = GetPrereleasePart()[1..];
        
			if (identifiersSpan.IsEmpty)
				return Array.Empty<string>();
            
			int dotCount = identifiersSpan.Count('.');
			string[] identifiers = new string[dotCount + 1];
        
			if (dotCount == 0)
			{
				identifiers[0] = identifiersSpan.ToString();
				return identifiers;
			}
        
			int previousStart = 0;
			int currentIndex = 0;
        
			for (int i = 0; i < dotCount; i++)
			{
				int nextDot = identifiersSpan[previousStart..].IndexOf('.');
				if (nextDot == -1) break;
            
				identifiers[currentIndex++] = identifiersSpan.Slice(previousStart, nextDot).ToString();
				previousStart += nextDot + 1;
			}
			
			if (previousStart < identifiersSpan.Length)
			{
				identifiers[currentIndex] = identifiersSpan[previousStart..].ToString();
			}
        
			return identifiers;
		}
	}
	
	public IList<string> MetaIdentifiers
	{
		get
		{
			if (!IsContainsMeta) return Array.Empty<string>();
        
			ReadOnlySpan<char> identifiersSpan = GetMetaPart()[1..];
        
			if (identifiersSpan.IsEmpty) 
				return Array.Empty<string>();
            
			int dotCount = identifiersSpan.Count('.');
			string[] identifiers = new string[dotCount + 1];
        
			if (dotCount == 0)
			{
				identifiers[0] = identifiersSpan.ToString();
				return identifiers;
			}
        
			int previousStart = 0;
			int currentIndex = 0;
        
			for (int i = 0; i < dotCount; i++)
			{
				int nextDot = identifiersSpan[previousStart..].IndexOf('.');
				if (nextDot == -1) break;
            
				identifiers[currentIndex++] = identifiersSpan.Slice(previousStart, nextDot).ToString();
				previousStart += nextDot + 1;
			}
			
			if (previousStart < identifiersSpan.Length)
			{
				identifiers[currentIndex] = identifiersSpan[previousStart..].ToString();
			}
        
			return identifiers;
		}
	}
	
	public SemVer(uint major, uint minor, uint patch)
	{
		_major = major;
		_minor = minor;
		_patch = patch;
		
		_prereleasePosition = _metaPosition = -1;
	}

	private SemVer(uint major, uint minor, uint patch, string? tail, int prereleasePosition, int metaPosition)
	{
		_major = major;
		_minor = minor;
		_patch = patch;
		_tail = tail;
		_prereleasePosition = prereleasePosition;
		_metaPosition = metaPosition;
	}

	public bool Equals(SemVer other)
	{
		return CompareTo(other) == 0;
	}

	public int CompareTo(SemVer other)
	{
		if (_major != other._major) return _major.CompareTo(other._major);
		if (_minor != other._minor) return _minor.CompareTo(other._minor);
		if (_patch != other._patch) return _patch.CompareTo(other._patch);

		IList<string> left = PrereleaseIdentifiers;
		IList<string> right = other.PrereleaseIdentifiers;

		int lesserPrereleaseIdentifiers = int.Min(left.Count, right.Count);

		for (int i = 0; i < lesserPrereleaseIdentifiers; i++)
		{
			int diff = string.Compare(left[i], right[i], StringComparison.Ordinal);
			
			if (diff == 0) continue;
			
			return diff;
		}

		return -left.Count.CompareTo(right.Count);
	}

	private static int CompareToOnlyTail(ReadOnlySpan<char> left, ReadOnlySpan<char> right)
	{
		return left.CompareTo(right, StringComparison.Ordinal);
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is SemVer semver && Equals(semver);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(_major, _minor, _patch, _prereleasePosition, _metaPosition);
	}

	public override string ToString()
	{
		return $"{_major}.{_minor}.{_patch}{_tail}";
	}

	private ReadOnlySpan<char> GetPrereleasePart()
	{
		if (_metaPosition >= 0)
		{
			return _tail.AsSpan(_prereleasePosition, _metaPosition - _prereleasePosition);
		}
		else
		{
			return _tail.AsSpan(_prereleasePosition);
		}
	}
	
	private ReadOnlySpan<char> GetMetaPart()
	{
		return _tail.AsSpan(_metaPosition);
	}

	public static bool operator ==(SemVer left, SemVer right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(SemVer left, SemVer right)
	{
		return !left.Equals(right);
	}

	public static bool operator >(SemVer left, SemVer right)
	{
		return left.CompareTo(right) > 0;
	}

	public static bool operator >=(SemVer left, SemVer right)
	{
		return left.CompareTo(right) >= 0;
	}

	public static bool operator <(SemVer left, SemVer right)
	{
		return left.CompareTo(right) < 0;
	}

	public static bool operator <=(SemVer left, SemVer right)
	{
		return left.CompareTo(right) <= 0;
	}
	
	public static SemVer Parse(string s)
		=> Parse(s, null);
	
    public static SemVer Parse(string s, IFormatProvider? provider)
    {
        return Parse(s.AsSpan(), provider);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, out SemVer result)
	    => TryParse(s, null, out result);

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out SemVer result)
    {
        if (s == null)
        {
            result = default;
            return false;
        }
        return TryParse(s.AsSpan(), provider, out result);
    }

    public static SemVer Parse(ReadOnlySpan<char> s)
	    => Parse(s, null);

    public static SemVer Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (TryParse(s, provider, out SemVer result))
        {
            return result;
        }
        ThrowHelper.ThrowFormatException("Invalid SemVer format");
        return default;
    }
    
    public static bool TryParse(ReadOnlySpan<char> s, out SemVer result)
		=> TryParse(s, null, out result);

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out SemVer result)
    {
        result = default;
        int index = 0;

        if (!TryParseNumber(s, ref index, out uint major))
            return false;

        if (index >= s.Length || s[index] != '.')
            return false;
        index++;

        if (!TryParseNumber(s, ref index, out uint minor))
            return false;

        if (index >= s.Length || s[index] != '.')
            return false;
        index++;

        if (!TryParseNumber(s, ref index, out uint patch))
            return false;

        string? tail = null;
        int prereleasePosition = -1;
        int metaPosition = -1;

        if (index < s.Length)
        {
            char c = s[index];
            if (c == '-')
            {
                if (index + 1 >= s.Length)
                    return false;

                prereleasePosition = 0;
                int plusIndex = s.Slice(index + 1).IndexOf('+');
                if (plusIndex >= 0)
                {
                    metaPosition = plusIndex + 1;
                }
                tail = s[index..].ToString();
            }
            else if (c == '+')
            {
                if (index + 1 >= s.Length)
                    return false;

                metaPosition = 0;
                tail = s[index..].ToString();
            }
            else
            {
                return false;
            }
        }

        result = new SemVer(major, minor, patch, tail, prereleasePosition, metaPosition);

        return result.PrereleaseIdentifiers.All(CheckIdentifier) && result.MetaIdentifiers.All(CheckIdentifier);
    }
	
	private static bool TryParseNumber(ReadOnlySpan<char> s, ref int index, out uint value)
	{
		// Have no clue what's going on here; written by DeepSeek
		value = 0;
		if (index >= s.Length || !char.IsDigit(s[index]))
			return false;

		if (s[index] == '0')
		{
			index++;
			if (index < s.Length && char.IsDigit(s[index]))
			{
				return false;
			}
			value = 0;
			return true;
		}

		while (index < s.Length && char.IsDigit(s[index]))
		{
			uint digit = (uint)(s[index] - '0');
			if (value > (uint.MaxValue - digit) / 10)
			{
				return false;
			}
			value = value * 10 + digit;
			index++;
		}
		return true;
	}

	private static bool CheckIdentifier(string identifier)
	{
		if (identifier.Length == 0) return false;

		int i = 0;
		if (identifier.Length > 1 && identifier[0] == '0')
		{
			if (char.IsAsciiDigit(identifier[1])) return false;
			i = 1;
		}

		for (; i < identifier.Length; i++)
		{
			char c = identifier[i];
			if (!char.IsDigit(c) && !char.IsAsciiLetter(c) && c != '-') return false;
		}

		return true;
	}
}