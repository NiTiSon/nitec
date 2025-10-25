using System.Collections.Generic;

namespace NiteLang.Metadata;

public abstract class NlrModule : INlrContainer, INlrMember
{
	public string Name { get; }
	public abstract INlrContainer Container { get; }
	public abstract IEnumerable<INlrMember> Members { get; }

	private protected NlrModule(string name)
	{
		Name = name;
	}
}