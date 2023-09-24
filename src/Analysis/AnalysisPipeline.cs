using System;
using System.Collections.Generic;
using System.IO;
using NiteCompiler.Analysis.Text;
using NiteCompiler.Analysis.Tokens;

namespace NiteCompiler.Analysis;

public class AnalysisPipeline
{
    private readonly string filePath;
    private CodeSource? source;
    private DiagnosticBag bag;
    internal List<Token>? tokens;

#if DEBUG
    internal long MeasureTicks;
    internal double MeasureSeconds => MeasureTicks / (double)TimeSpan.TicksPerSecond;
#endif

    public AnalysisPipeline(string filePath)
    {
        this.filePath = filePath;
        bag = new DiagnosticBag();
    }

    /// <summary>
    /// Read file content.
    /// </summary>
    public void ReadFile()
    {
        try
        {
            using FileStream fs = File.OpenRead(filePath);
            using TextReader tr = new StreamReader(fs);

            source = new(tr.ReadToEnd(), filePath);
        }
        catch (Exception ex)
        {
            bag.ReportError(default, "NC0001", $"Error during file reading: {ex.GetType().FullName}, message: '{ex.Message}'");
        }
    }
    public void Tokenize()
    {
        Lexer lexer = new(LanguageOptions.StandardV1, source!, bag);

        tokens = new();
        while (!lexer.IsEOF)
        {
            tokens.Add(lexer.NextToken());
        }
    }
    #if DEBUG
    internal void WriteTokens()
    {
        if (tokens is null)
        {
            Nitec.CriticalExitWithError($"Tokens are not generated yet for file '{Path.GetFullPath(filePath)}'!");
        }

        using FileStream fs = File.OpenWrite(filePath + ".tokens");
        using TextWriter tw = new StreamWriter(fs);

        tw.WriteLine($"Tokens of file '{Path.GetFullPath(filePath)}':");
        foreach (Token token in tokens!)
        {
            tw.WriteLine(token);
        }
    }
    #endif
}