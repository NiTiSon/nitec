namespace NiteCompiler.CodeAnalysis.Binding.Pure;

public enum Pureness
{
	/// <summary>
	/// The expression or symbol is not pure.
	/// </summary>
	None = 0,

	/// <summary>
	/// The expression or symbol is pure only with determined target.
	/// </summary>
	TargetPure = 1,

	/// <summary>
	/// The expression or symbol is pure.
	/// </summary>
	Pure = 2,
}

public static class PurenessExtensions
{
	extension(ref Pureness pureness)
	{
		public void operator +=(Pureness other)
		{
			pureness = (Pureness)int.Min((int)pureness, (int)other);
		}
	}
}