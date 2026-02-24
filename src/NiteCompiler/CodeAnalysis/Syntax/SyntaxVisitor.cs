namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class SyntaxVisitor
{
	public virtual void Visit(SyntaxNode? node)
	{
		node?.Accept(this);
	}

	protected virtual void DefaultVisit(SyntaxNode node)
	{
	}

	public virtual void VisitCompilationUnit(CompilationUnitSyntax node) => DefaultVisit(node);
	public virtual void VisitToken(Token token) => DefaultVisit(token);
	public virtual void VisitTrivia(Trivia trivia) => DefaultVisit(trivia);
	public virtual void VisitSyntaxList<T>(SyntaxList<T> list) where T : SyntaxNode  => DefaultVisit(list);

	public virtual void VisitUnaryExpression(UnaryExpressionSyntax expression) => DefaultVisit(expression);
	public virtual void VisitBinaryExpression(BinaryExpressionSyntax expression) => DefaultVisit(expression);
	public virtual void VisitAssignmentExpression(AssignmentExpressionSyntax expression) => DefaultVisit(expression);
	public virtual void VisitParenthesizedExpression(ParenthesizedExpressionSyntax expression) => DefaultVisit(expression);
	public virtual void VisitLiteralExpression(LiteralExpressionSyntax expression) => DefaultVisit(expression);
	public virtual void VisitSimpleName(SimpleNameSyntax expression) => DefaultVisit(expression);
	public virtual void VisitPredefinedType(PredefinedTypeSyntax expression) => DefaultVisit(expression);

	public virtual void VisitModuleDeclaration(ModuleDeclarationSyntax declaration) => DefaultVisit(declaration);
	public virtual void VisitFunctionDeclaration(FunctionDeclarationSyntax declaration) => DefaultVisit(declaration);
	public virtual void VisitBlockFunctionBody(BlockFunctionBodySyntax body) => DefaultVisit(body);
	public virtual void VisitEmptyFunctionBody(EmptyFunctionBodySyntax body) => DefaultVisit(body);
	// public virtual TResult? VisitTypeDeclaration(TypeDeclarationSyntax declaration) => DefaultVisit(declaration);
	public virtual void VisitMembersTypeBody(MembersTypeBodySyntax body) => DefaultVisit(body);
	public virtual void VisitEmptyTypeBody(EmptyTypeBodySyntax body) => DefaultVisit(body);
	public virtual void VisitLocalVariableDeclarator(LocalVariableDeclarator declarator) => DefaultVisit(declarator);
	public virtual void VisitEqualsValueClause(EqualsValueClause clause) => DefaultVisit(clause);
	public virtual void VisitTypeClause(TypeClause clause) => DefaultVisit(clause);

	public virtual void VisitEmptyStatement(EmptyStatementSyntax statement) => DefaultVisit(statement);
	public virtual void VisitReturnStatement(ReturnStatementSyntax statement) => DefaultVisit(statement);
	public virtual void VisitExpressionStatement(ExpressionStatementSyntax statement) => DefaultVisit(statement);
	public virtual void VisitLocalVariableDeclarationStatement(LocalVariableDeclarationStatement statement) => DefaultVisit(statement);
	public virtual void VisitBlockStatement(BlockStatementSyntax statement) => DefaultVisit(statement);
}

public abstract class SyntaxVisitor<TResult>
{
	public virtual TResult? Visit(SyntaxNode? node)
	{
		return node != null ? node.Accept(this) : default;
	}

	protected virtual TResult? DefaultVisit(SyntaxNode node) => default;

	public virtual TResult? VisitCompilationUnit(CompilationUnitSyntax node) => DefaultVisit(node);
	public virtual TResult? VisitToken(Token token) => DefaultVisit(token);
	public virtual TResult? VisitTrivia(Trivia trivia) => DefaultVisit(trivia);
	public virtual TResult? VisitSyntaxList<T>(SyntaxList<T> list) where T : SyntaxNode  => DefaultVisit(list);

	public virtual TResult? VisitUnaryExpression(UnaryExpressionSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitBinaryExpression(BinaryExpressionSyntax expression)  => DefaultVisit(expression);
	public virtual TResult? VisitAssignmentExpression(AssignmentExpressionSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitParenthesizedExpression(ParenthesizedExpressionSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitLiteralExpression(LiteralExpressionSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitSimpleName(SimpleNameSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitPredefinedType(PredefinedTypeSyntax expression) => DefaultVisit(expression);

	public virtual TResult? VisitModuleDeclaration(ModuleDeclarationSyntax declaration) => DefaultVisit(declaration);
	public virtual TResult? VisitFunctionDeclaration(FunctionDeclarationSyntax declaration) => DefaultVisit(declaration);
	public virtual TResult? VisitBlockFunctionBody(BlockFunctionBodySyntax body) => DefaultVisit(body);
	public virtual TResult? VisitEmptyFunctionBody(EmptyFunctionBodySyntax body) => DefaultVisit(body);
	// public virtual TResult? VisitTypeDeclaration(TypeDeclarationSyntax declaration) => DefaultVisit(declaration);
	public virtual TResult? VisitMembersTypeBody(MembersTypeBodySyntax body) => DefaultVisit(body);
	public virtual TResult? VisitEmptyTypeBody(EmptyTypeBodySyntax body) => DefaultVisit(body);
	public virtual TResult? VisitLocalVariableDeclarator(LocalVariableDeclarator declarator) => DefaultVisit(declarator);
	public virtual TResult? VisitEqualsValueClause(EqualsValueClause clause) => DefaultVisit(clause);
	public virtual TResult? VisitTypeClause(TypeClause clause) => DefaultVisit(clause);

	public virtual TResult? VisitEmptyStatement(EmptyStatementSyntax statement) => DefaultVisit(statement);
	public virtual TResult? VisitReturnStatement(ReturnStatementSyntax statement) => DefaultVisit(statement);
	public virtual TResult? VisitExpressionStatement(ExpressionStatementSyntax statement) => DefaultVisit(statement);
	public virtual TResult? VisitLocalVariableDeclarationStatement(LocalVariableDeclarationStatement statement) => DefaultVisit(statement);
	public virtual TResult? VisitBlockStatement(BlockStatementSyntax statement) => DefaultVisit(statement);
}