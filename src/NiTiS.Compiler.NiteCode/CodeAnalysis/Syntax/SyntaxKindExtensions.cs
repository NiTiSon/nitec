namespace NiTiS.Compiler.NiteCode.CodeAnalysis.Syntax;

public static class SyntaxKindExtensions
{
	/// <summary>
	/// Token is keyword, ether contextual or not.
	/// </summary>
	internal const uint KeywordFlag		= 0b1000_0000_0000_0000_0000_0000_0000_0000u;

	/// <summary>
	/// Token is contextual.
	/// </summary>
	internal const uint ContextualFlag	= 0b0100_0000_0000_0000_0000_0000_0000_0000u;

	/// <summary>
	/// Token is trivia.
	/// </summary>
	internal const uint TriviaFlag		= 0b0010_0000_0000_0000_0000_0000_0000_0000u;

	/// <summary>
	/// Token contains exclusive string content (e.g. StringLiteral, Identifier).
	/// </summary>
	internal const uint ValuableFlag	= 0b0001_0000_0000_0000_0000_0000_0000_0000u;

	/// <summary>
	/// Token is operator.
	/// </summary>
	internal const uint OperatorFlag	= 0b0000_1000_0000_0000_0000_0000_0000_0000u;

	/// <summary>
	/// Token is operator visible looks like 2+ other existed operators.
	/// </summary>
	internal const uint CompoundFlag	= 0b0000_0100_0000_0000_0000_0000_0000_0000u;

	/// <summary>
	/// Token is type keyword.
	/// </summary>
	internal const uint TypeFlag		= 0b0000_0010_0000_0000_0000_0000_0000_0000u;

	extension(SyntaxKind kind)
	{
		public uint Ordinal => (uint)kind & 0x0000FFFFu;

		public bool IsItKeyword => kind.HasFlag((SyntaxKind)KeywordFlag);
		public bool IsContextual => kind.HasFlag((SyntaxKind)ContextualFlag);
		public bool IsTrivia => kind.HasFlag((SyntaxKind)TriviaFlag);
		public bool IsValuable => kind.HasFlag((SyntaxKind)ValuableFlag);
		public bool IsOperator => kind.HasFlag((SyntaxKind)OperatorFlag);
		public bool IsCompound => kind.HasFlag((SyntaxKind)CompoundFlag);
		public bool IsType => kind.HasFlag((SyntaxKind)TypeFlag);
	}
}