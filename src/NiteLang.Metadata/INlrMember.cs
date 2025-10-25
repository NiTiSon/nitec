namespace NiteLang.Metadata;

public interface INlrMember
{
	string Name { get; }
	INlrContainer Container { get; }
}
