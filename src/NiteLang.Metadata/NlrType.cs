using System;
using System.Collections.Generic;
using System.Text;

namespace NiteLang.Metadata;

public abstract class NlrType : INlrContainer, INlrMember
{
	public string Name { get; }
	public abstract INlrContainer Container { get; }
	public abstract IEnumerable<INlrMember> Members { get; }
	public abstract NlrTypeFlags Flags { get; }
	public abstract NlrType? BaseType { get; }

	private protected NlrType(string name)
	{
		Name = name;
	}
}