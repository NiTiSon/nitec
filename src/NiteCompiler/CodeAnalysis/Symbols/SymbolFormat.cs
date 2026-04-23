using System;

namespace NiteCompiler.CodeAnalysis.Symbols;

[Flags]
public enum SymbolFormat : byte
{
	/// <summary>
	/// Use short names for a range of special types if possible.
	/// </summary>
	PreferShortSpecialTypeName = 1 << 0,

	/// <summary>
	/// Include [libname] at the beggining.
	/// </summary>
	IncludeLibrary = 1 << 1,

	/// <summary>
	/// Emit global module name.
	/// </summary>
	EmitGlobalModule = 1 << 2,

	/// <summary>
	/// Omit module path.
	/// <example>
	/// <c>std::numerics</c> will return just <c>numerics</c>.
	/// </example>
	/// </summary>
	OmitModulePath = 1 << 3,

	/// <summary>
	/// Omit container that owns symbol.
	/// <example>
	/// <c>module::MyType.my_field</c> will return <c>my_field</c>.
	/// </example>
	/// </summary>
	/// <remarks>
	/// Only valid with <see cref="OmitModulePath"/>.
	/// </remarks>
	OmitContainer = 1 << 4,

	/// <summary>
	/// Omit parameter names, leaving only parameter types.
	/// </summary>
	OmitParameterNames = 1 << 5,

	Detailed = EmitGlobalModule,
	Default = 0,
}

public static class SymbolFormatExtensions
{
	extension(SymbolFormat format)
	{
		public bool IsValid
		{
			get
			{
				bool valid = true;

				if (format.HasFlag(SymbolFormat.OmitContainer) && !format.HasFlag(SymbolFormat.OmitModulePath))
				{
					valid = false;
				}

				return valid;
			}
		}
	}
}