using System.Collections.Generic;

namespace NiteLang.Metadata;

public abstract class NlrFunction : INlrMember
{
	public string Name { get; }
	public INlrContainer Container { get; }
	public abstract NlrFunctionFlags Flags { get; }
	public abstract IEnumerable<NlrParameter> Parameters { get; }
	public abstract NlrType ReturnParameter { get; }
	public abstract ExecutionArchitecture Architecture { get; }
	public abstract byte[]? Body { get; }

	private protected NlrFunction(string name)
	{
		Name = name;
	}
}