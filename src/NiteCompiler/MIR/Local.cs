namespace NiteCompiler.MIR;

public abstract class Local
{
	public readonly LocalId Id;

	public Local(LocalId id)
	{
		Id = id;
	}
}