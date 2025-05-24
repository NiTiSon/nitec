using System.Collections;
using System.Collections.Generic;

namespace Nlr.CodeGen;

public abstract class Module : IEnumerable<IModuleMember>
{
	/// <summary>
	/// Full name of module.
	/// </summary>
	/// <remarks>
	/// Null, if module is global.
	/// </remarks>
	public abstract string? Name { get; }

	public abstract IEnumerator<IModuleMember> GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}