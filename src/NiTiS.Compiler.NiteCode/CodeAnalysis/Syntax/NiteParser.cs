using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NiteCompiler.CodeAnalysis.Text;
using NiTiS.Compiler.CodeAnalysis.Syntax;
using NiTiS.Compiler.Diagnostics;
using NiTiS.Compiler.NiteCode.CodeAnalysis.Syntax.Expressions;

namespace NiTiS.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class NiteParser
{
	private readonly List<NiteToken> _tokens;

	public NiteParser(DiagnosticBag diagnostics, SourceText source)
	{
		NiteLexer lexer = new(diagnostics, source);

		_tokens = new(source.Length / 8);
		NiteToken token = lexer.Lex();

		while (token.Kind != SyntaxKind.EndOfFile)
		{
			_tokens.Add(token);
			token = lexer.Lex();
		}
	}

	public NiteCompilationUnit ParseCompilationUnit()
	{
		throw new NotImplementedException();
	}

	public BinaryExpressionSyntax ParseBinaryExpression()
	{
		throw new NotImplementedException();
	}
}