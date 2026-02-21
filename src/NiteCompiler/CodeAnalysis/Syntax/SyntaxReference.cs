using System.Threading;
using System.Threading.Tasks;
using NiteCompiler.CodeAnalysis.Text;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class SyntaxReference
{
	public abstract SyntaxTree SyntaxTree { get; }

	public abstract TextSpan Span { get; }

	public abstract SyntaxNode GetSyntax(CancellationToken cancellationToken = default);

	public virtual Task<SyntaxNode> GetSyntaxAsync(CancellationToken cancellationToken = default)
	{
		return Task.FromResult(GetSyntax(cancellationToken));
	}

	internal SourceLocation GetLocation()
	{
		SourceLocation location = new(SyntaxTree, Span);
		return location;
	}
}