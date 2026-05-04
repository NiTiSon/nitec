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
	public virtual void VisitGenericParameterList(GenericParameterListSyntax list) => DefaultVisit(list);

	public virtual void VisitUnaryExpression(UnaryExpressionSyntax expression) => DefaultVisit(expression);
	public virtual void VisitBinaryExpression(BinaryExpressionSyntax expression) => DefaultVisit(expression);
	public virtual void VisitAssignmentExpression(AssignmentExpressionSyntax expression) => DefaultVisit(expression);
	public virtual void VisitParenthesizedExpression(ParenthesizedExpressionSyntax expression) => DefaultVisit(expression);
	public virtual void VisitLiteralExpression(LiteralExpressionSyntax expression) => DefaultVisit(expression);
	public virtual void VisitIdentifierName(IdentifierNameSyntax expression) => DefaultVisit(expression);
	public virtual void VisitGenericName(GenericNameSyntax expression) => DefaultVisit(expression);
	public virtual void VisitPathName(PathNameSyntax expression) => DefaultVisit(expression);
	public virtual void VisitPredefinedType(PredefinedTypeSyntax expression) => DefaultVisit(expression);
	public virtual void VisitPointerType(TypeSyntax expression) => DefaultVisit(expression);
	public virtual void VisitReferenceType(ReferenceTypeSyntax expression) => DefaultVisit(expression);
	public virtual void VisitOptionalType(TypeSyntax expression) => DefaultVisit(expression);
	public virtual void VisitInvocationExpression(InvocationExpressionSyntax expression) => DefaultVisit(expression);
	public virtual void VisitIndexationExpression(IndexationExpressionSyntax expression) => DefaultVisit(expression);

	public virtual void VisitModuleDeclaration(ModuleDeclarationSyntax declaration) => DefaultVisit(declaration);
	public virtual void VisitFunctionDeclaration(FunctionDeclarationSyntax declaration) => DefaultVisit(declaration);
	public virtual void VisitParameterList(ParameterListSyntax list) => DefaultVisit(list);
	public virtual void VisitParameter(ParameterSyntax parameter) => DefaultVisit(parameter);
	public virtual void VisitArgumentList(ArgumentListSyntax list) => DefaultVisit(list);
	public virtual void VisitBracketedArgumentList(BracketedArgumentListSyntax list) => DefaultVisit(list);
	public virtual void VisitBlockFunctionBody(BlockFunctionBodySyntax body) => DefaultVisit(body);
	public virtual void VisitEmptyFunctionBody(EmptyFunctionBodySyntax body) => DefaultVisit(body);
	public virtual void VisitTypeDeclaration(TypeDeclarationSyntax declaration) => DefaultVisit(declaration);
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
	public virtual void VisitIfStatement(IfStatementSyntax statement) => DefaultVisit(statement);
	public virtual void VisitLoopStatement(LoopStatementSyntax statement) => DefaultVisit(statement);
	public virtual void VisitWhileStatement(WhileStatementSyntax statement) => DefaultVisit(statement);
	public virtual void VisitElseClause(ElseClauseSyntax elseClause) => DefaultVisit(elseClause);
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
	public virtual TResult? VisitGenericParameterList(GenericParameterListSyntax list) => DefaultVisit(list);

	public virtual TResult? VisitUnaryExpression(UnaryExpressionSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitBinaryExpression(BinaryExpressionSyntax expression)  => DefaultVisit(expression);
	public virtual TResult? VisitAssignmentExpression(AssignmentExpressionSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitParenthesizedExpression(ParenthesizedExpressionSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitLiteralExpression(LiteralExpressionSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitIdentifierName(IdentifierNameSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitGenericName(GenericNameSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitPathName(PathNameSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitPredefinedType(PredefinedTypeSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitPointerType(TypeSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitReferenceType(ReferenceTypeSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitOptionalType(TypeSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitInvocationExpression(InvocationExpressionSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitIndexationExpression(IndexationExpressionSyntax expression) => DefaultVisit(expression);

	public virtual TResult? VisitModuleDeclaration(ModuleDeclarationSyntax declaration) => DefaultVisit(declaration);
	public virtual TResult? VisitFunctionDeclaration(FunctionDeclarationSyntax declaration) => DefaultVisit(declaration);
	public virtual TResult? VisitParameterList(ParameterListSyntax list) => DefaultVisit(list);
	public virtual TResult? VisitParameter(ParameterSyntax parameter) => DefaultVisit(parameter);
	public virtual TResult? VisitArgumentList(ArgumentListSyntax list) => DefaultVisit(list);
	public virtual TResult? VisitBracketedArgumentList(BracketedArgumentListSyntax list) => DefaultVisit(list);
	public virtual TResult? VisitBlockFunctionBody(BlockFunctionBodySyntax body) => DefaultVisit(body);
	public virtual TResult? VisitEmptyFunctionBody(EmptyFunctionBodySyntax body) => DefaultVisit(body);
	public virtual TResult? VisitTypeDeclaration(TypeDeclarationSyntax declaration) => DefaultVisit(declaration);
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
	public virtual TResult? VisitIfStatement(IfStatementSyntax statement) => DefaultVisit(statement);
	public virtual TResult? VisitLoopStatement(LoopStatementSyntax statement) => DefaultVisit(statement);
	public virtual TResult? VisitWhileStatement(WhileStatementSyntax statement) => DefaultVisit(statement);
	public virtual TResult? VisitElseClause(ElseClauseSyntax elseClause) => DefaultVisit(elseClause);
}