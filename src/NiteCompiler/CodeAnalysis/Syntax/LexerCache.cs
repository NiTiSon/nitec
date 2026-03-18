using System.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

internal sealed class LexerCache
{
	private static readonly ObjectPool<LexerCache> CachePool = new(static () => new());

	private const int LeadingTriviaCacheInitialCapacity = 128;
	private const int TrailingTriviaCacheInitialCapacity = 16;

	private SyntaxList<Trivia>.Builder? _leadingTriviaCache;
	private SyntaxList<Trivia>.Builder? _trailingTriviaCache;
	private StringBuilder? _builder;

	public static LexerCache GetInstance()
	{
		return CachePool.Allocate();
	}

	public void Free()
	{
		CachePool.Free(this);

		if (_leadingTriviaCache?.Capacity > LeadingTriviaCacheInitialCapacity * 4)
		{
			_leadingTriviaCache = null;
		}

		if (_trailingTriviaCache?.Capacity > TrailingTriviaCacheInitialCapacity * 4)
		{
			_trailingTriviaCache = null;
		}
	}

	public SyntaxList<Trivia>.Builder LeadingTrivia
	{
		get
		{
			_leadingTriviaCache ??= new(LeadingTriviaCacheInitialCapacity);

			return _leadingTriviaCache;
		}
	}

	public SyntaxList<Trivia>.Builder TrailingTrivia
	{
		get
		{
			_trailingTriviaCache ??= new(TrailingTriviaCacheInitialCapacity);

			return _trailingTriviaCache;
		}
	}

	public StringBuilder StringBuilder
	{
		get
		{
			_builder ??= new(32);

			return _builder;
		}
	}
}