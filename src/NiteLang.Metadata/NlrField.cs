namespace NiteLang.Metadata;

public abstract class NlrField : INlrMember
{
	public string Name { get; }
	public abstract NlrFieldFlags Flags { get; }
	public abstract INlrContainer Container { get; }

	private protected NlrField(string name)
	{
		Name = name;
	}
}