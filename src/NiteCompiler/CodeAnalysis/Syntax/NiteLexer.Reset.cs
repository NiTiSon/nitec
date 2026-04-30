namespace NiteCompiler.CodeAnalysis.Syntax;

internal partial class NiteLexer
{
	private readonly ref struct ResetPoint(int position)
	{
		public readonly int Position = position;
	}

	private ResetPoint GetResetPoint()
	{
		return new ResetPoint(_window.Position);
	}

	private void Reset(ResetPoint rp)
	{
		_window.Reset(rp.Position);
	}
}