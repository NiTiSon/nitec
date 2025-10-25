using System.Collections.Generic;

namespace NiteLang.Metadata;

public abstract class NlrLibrary : INlrContainer
{
	public string Name { get; }
	public abstract IEnumerable<INlrMember> Members { get; }
	public abstract NlrLibrary Flags { get; }

	private protected NlrLibrary(string name)
	{
		Name = name;
	}
}