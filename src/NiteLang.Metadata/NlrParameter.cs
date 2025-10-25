namespace NiteLang.Metadata;

public abstract class NlrParameter
{
	public string Name { get; }
	public abstract NlrType Type { get; }
	public abstract NlrParameterFlags Flags { get; }

	private protected NlrParameter(string name)
	{
		Name = name;
	}
}