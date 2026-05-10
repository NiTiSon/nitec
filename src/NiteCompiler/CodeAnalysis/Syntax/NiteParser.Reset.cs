using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Syntax;

internal partial class NiteParser
{
	private DiagnosticBag? _storedActualDiagnosticBag;

	private readonly ref struct ResetPoint(int position)
	{
		public readonly int Position = position;
	}

	private ResetPoint GetResetPoint()
	{
		_storedActualDiagnosticBag = _diagnostics;
		_diagnostics = [];
		return new ResetPoint(_position);
	}

	private void UpdateResetPoint(ref ResetPoint rp)
	{
		ReleaseResetPoint(rp);
		rp = GetResetPoint();
	}

	private void Reset(ResetPoint rp)
	{
		_position = rp.Position;
		_diagnostics.Clear();
	}

	private void ReleaseResetPoint(ResetPoint rp)
	{
		_storedActualDiagnosticBag!.AddRange(_diagnostics);
		_diagnostics = _storedActualDiagnosticBag;
		_storedActualDiagnosticBag = null;
	}
}