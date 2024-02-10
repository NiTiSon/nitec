using NiteCode.Compiler.CodeAnalysis.Syntax;
using NiteCode.Compiler.CodeAnalysis.Text;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NiteCode.Compiler.CodeAnalysis;

public sealed class DiagnosticBag : IEnumerable<Diagnostic>
{
	private readonly List<Diagnostic> diagnostics;

	public DiagnosticBag()
	{
		diagnostics = [];

	}

	public IEnumerator<Diagnostic> GetEnumerator()
		=> diagnostics.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator()
		=> GetEnumerator();

	public void Add(Diagnostic diagnostic)
	{
		diagnostics.Add(diagnostic);
	}
	public void AddRange(IEnumerable<Diagnostic> diagnostics)
	{
		this.diagnostics.AddRange(diagnostics);
	}
	public void ReportInvalidCharacter(TextLocation location, char symbol)
		=> Add(Diagnostic.Create(NiteCodeDiagnosticsDescriptors.NC0001, location, symbol));
	public void ReportUnterminatedMultiLineComment(TextLocation location)
		=> Add(Diagnostic.Create(NiteCodeDiagnosticsDescriptors.NC0002, location));
	public void ReportUnexpectedToken(TextLocation location, SyntaxKind actualKind, SyntaxKind expectedKind)
		=> Add(Diagnostic.Create(NiteCodeDiagnosticsDescriptors.NC0003, location, actualKind, expectedKind));
	public void ReportMoreThanOneModuleDeclaration(TextLocation location)
		=> Add(Diagnostic.Create(NiteCodeDiagnosticsDescriptors.NC0004, location));
	public void ReportWrongModuleDeclarationLocation(TextLocation location)
		=> Add(Diagnostic.Create(NiteCodeDiagnosticsDescriptors.NC0005, location));
	public void ReportZeroByte(TextLocation location)
		=> Add(Diagnostic.Create(NiteCodeDiagnosticsDescriptors.NC0006, location));
}