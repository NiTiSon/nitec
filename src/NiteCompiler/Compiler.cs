using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.CodeAnalysis.Syntax.Directives;
using NiteCompiler.CodeAnalysis.Text;
using NiTiS.Compiler.Diagnostics;

namespace NiteCompiler;

public class Compiler
{
    private readonly PackageSymbol _package;
    private SymbolTable _symbolTable;

    private int _atom = 0;

    public Compiler(string packageName)
    {
        _package = new PackageSymbol(packageName);
        _symbolTable = new SymbolTable();
    }

    public void Compile(ReadOnlySpan<FileInfo> files, DiagnosticBag diagnostics, Stream output)
    {
        if (Interlocked.Exchange(ref _atom, 1) == 0)
        {
            try
            {
                foreach (var file in files)
                {
                    Console.WriteLine($"Compiling {file.FullName}");
                    NiteLexer lexer = new(new StringText(File.ReadAllText(file.FullName)), diagnostics);
                    NiteParser parser = new(lexer, diagnostics);

                    SyntaxTree tree = parser.Parse();

                    foreach (UseDirectiveSyntax directive in tree.Usages)
                    {
	                    SyntaxTree.PrintTree(directive, "", true);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new OperationCanceledException(
                    "Internal compilation exception. Please report this problem to compiler developers!!!",
                    ex);
            }
            finally
            {
                Interlocked.Exchange(ref _atom, 0);
            }
        }
        else
        {
            throw new InvalidOperationException("Compilation process is already running.");
        }
    }
}