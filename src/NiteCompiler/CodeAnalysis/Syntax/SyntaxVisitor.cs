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
	public virtual void VisitLifetimeParameter(LifetimeSyntax lifetime) => DefaultVisit(lifetime);
	public virtual void VisitGenericParameter(GenericParameterSyntax parameter) => DefaultVisit(parameter);
	public virtual void VisitGenericParameterList(LifetimeAndGenericParameterListSyntax list) => DefaultVisit(list);
	public virtual void VisitConstraint(ConstraintSyntax constraint) => DefaultVisit(constraint);

	public virtual void VisitUnaryExpression(UnaryExpressionSyntax expression) => DefaultVisit(expression);
	public virtual void VisitBinaryExpression(BinaryExpressionSyntax expression) => DefaultVisit(expression);
	public virtual void VisitAssignmentExpression(AssignmentExpressionSyntax expression) => DefaultVisit(expression);
	public virtual void VisitParenthesizedExpression(ParenthesizedExpressionSyntax expression) => DefaultVisit(expression);
	public virtual void VisitLiteralExpression(LiteralExpressionSyntax expression) => DefaultVisit(expression);
	public virtual void VisitName(NameSyntax expression) => DefaultVisit(expression);
	public virtual void VisitPredefinedType(PredefinedTypeSyntax expression) => DefaultVisit(expression);
	public virtual void VisitPointerType(TypeSyntax expression) => DefaultVisit(expression);
	public virtual void VisitReferenceType(ReferenceTypeSyntax expression) => DefaultVisit(expression);
	public virtual void VisitOptionalType(TypeSyntax expression) => DefaultVisit(expression);
	public virtual void VisitInvocationExpression(InvocationExpressionSyntax expression) => DefaultVisit(expression);
	public virtual void VisitIndexationExpression(IndexationExpressionSyntax expression) => DefaultVisit(expression);
	public virtual void VisitSelfExpression(SelfExpressionSyntax expression) => DefaultVisit(expression);
	public virtual void VisitMemberAccessExpression(MemberAccessExpressionSyntax expression) => DefaultVisit(expression);
	public virtual void VisitArrayType(ArrayTypeSyntax type) => DefaultVisit(type);
	public virtual void VisitUnsizedArrayType(UnsizedArrayTypeSyntax type) => DefaultVisit(type);
	public virtual void VisitNeverType(NeverTypeSyntax type) => DefaultVisit(type);
	public virtual void VisitCastExpression(CastExpressionSyntax expression) => DefaultVisit(expression);

	public virtual void VisitModuleDeclaration(ModuleDeclarationSyntax declaration) => DefaultVisit(declaration);
	public virtual void VisitFunctionDeclaration(FunctionDeclarationSyntax declaration) => DefaultVisit(declaration);
	public virtual void VisitParameterList(ParameterListSyntax list) => DefaultVisit(list);
	public virtual void VisitParameter(ParameterSyntax parameter) => DefaultVisit(parameter);
	public virtual void VisitArgumentList(ArgumentListSyntax list) => DefaultVisit(list);
	public virtual void VisitBracketedArgumentList(BracketedArgumentListSyntax list) => DefaultVisit(list);
	public virtual void VisitFunctionBody(FunctionBodySyntax body) => DefaultVisit(body);
	public virtual void VisitTypeDeclaration(TypeDeclarationSyntax declaration) => DefaultVisit(declaration);
	public virtual void VisitFieldDeclaration(FieldDeclarationSyntax declaration) => DefaultVisit(declaration);
	public virtual void VisitConstructorDeclaration(BaseConstructorDeclarationSyntax declaration) => DefaultVisit(declaration);
	public virtual void VisitGenerativeParameter(GenerativeParameterSyntax parameter) => DefaultVisit(parameter);
	public virtual void VisitTypeBody(TypeBodySyntax body) => DefaultVisit(body);
	public virtual void VisitLocalVariableDeclarator(LocalVariableDeclarator declarator) => DefaultVisit(declarator);
	public virtual void VisitEqualsValueClause(EqualsValueClause clause) => DefaultVisit(clause);
	public virtual void VisitTypeClause(TypeClauseSyntax clauseSyntax) => DefaultVisit(clauseSyntax);

	public virtual void VisitEmptyStatement(EmptyStatementSyntax statement) => DefaultVisit(statement);
	public virtual void VisitReturnStatement(ReturnStatementSyntax statement) => DefaultVisit(statement);
	public virtual void VisitExpressionStatement(ExpressionStatementSyntax statement) => DefaultVisit(statement);
	public virtual void VisitLocalVariableDeclarationStatement(LocalVariableDeclarationStatement statement) => DefaultVisit(statement);
	public virtual void VisitBlockStatement(BlockStatementSyntax statement) => DefaultVisit(statement);
	public virtual void VisitIfStatement(IfStatementSyntax statement) => DefaultVisit(statement);
	public virtual void VisitLoopStatement(LoopStatementSyntax statement) => DefaultVisit(statement);
	public virtual void VisitWhileStatement(WhileStatementSyntax statement) => DefaultVisit(statement);
	public virtual void VisitBreakStatement(BreakStatementSyntax statement) => DefaultVisit(statement);
	public virtual void VisitErrorStatement(ErrorStatementSyntax statement) => DefaultVisit(statement);
	public virtual void VisitElseClause(ElseClauseSyntax elseClause) => DefaultVisit(elseClause);
	public virtual void VisitLifetimeOrGenericConstraintClause(LifetimeOrGenericConstraintClauseSyntax elseClause) => DefaultVisit(elseClause);
	public virtual void VisitAttribute(AttributeSyntax attribute) => DefaultVisit(attribute);
	public virtual void VisitAttributeList(AttributeListSyntax attributeList) => DefaultVisit(attributeList);
	public virtual void VisitAttributeDeclaration(AttributeDeclarationSyntax declaration) => DefaultVisit(declaration);
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
	public virtual TResult? VisitLifetimeParameter(LifetimeSyntax lifetime) => DefaultVisit(lifetime);
	public virtual TResult? VisitGenericParameter(GenericParameterSyntax parameter) => DefaultVisit(parameter);
	public virtual TResult? VisitGenericParameterList(LifetimeAndGenericParameterListSyntax list) => DefaultVisit(list);
	public virtual TResult? VisitConstraint(ConstraintSyntax constraint) => DefaultVisit(constraint);

	public virtual TResult? VisitUnaryExpression(UnaryExpressionSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitBinaryExpression(BinaryExpressionSyntax expression)  => DefaultVisit(expression);
	public virtual TResult? VisitAssignmentExpression(AssignmentExpressionSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitParenthesizedExpression(ParenthesizedExpressionSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitLiteralExpression(LiteralExpressionSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitName(NameSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitPredefinedType(PredefinedTypeSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitPointerType(TypeSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitReferenceType(ReferenceTypeSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitOptionalType(TypeSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitInvocationExpression(InvocationExpressionSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitIndexationExpression(IndexationExpressionSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitSelfExpression(SelfExpressionSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitMemberAccessExpression(MemberAccessExpressionSyntax expression) => DefaultVisit(expression);
	public virtual TResult? VisitArrayType(ArrayTypeSyntax type) => DefaultVisit(type);
	public virtual TResult? VisitUnsizedArrayType(UnsizedArrayTypeSyntax type) => DefaultVisit(type);
	public virtual TResult? VisitNeverType(NeverTypeSyntax type) => DefaultVisit(type);
	public virtual TResult? VisitCastExpression(CastExpressionSyntax expression) => DefaultVisit(expression);

	public virtual TResult? VisitModuleDeclaration(ModuleDeclarationSyntax declaration) => DefaultVisit(declaration);
	public virtual TResult? VisitFunctionDeclaration(FunctionDeclarationSyntax declaration) => DefaultVisit(declaration);
	public virtual TResult? VisitParameterList(ParameterListSyntax list) => DefaultVisit(list);
	public virtual TResult? VisitParameter(ParameterSyntax parameter) => DefaultVisit(parameter);
	public virtual TResult? VisitArgumentList(ArgumentListSyntax list) => DefaultVisit(list);
	public virtual TResult? VisitBracketedArgumentList(BracketedArgumentListSyntax list) => DefaultVisit(list);
	public virtual TResult? VisitFunctionBody(FunctionBodySyntax body) => DefaultVisit(body);
	public virtual TResult? VisitTypeDeclaration(TypeDeclarationSyntax declaration) => DefaultVisit(declaration);
	public virtual TResult? VisitFieldDeclaration(FieldDeclarationSyntax declaration) => DefaultVisit(declaration);
	public virtual TResult? VisitConstructorDeclaration(BaseConstructorDeclarationSyntax declaration) => DefaultVisit(declaration);
	public virtual TResult? VisitGenerativeParameter(GenerativeParameterSyntax parameter) => DefaultVisit(parameter);
	public virtual TResult? VisitTypeBody(TypeBodySyntax body) => DefaultVisit(body);
	public virtual TResult? VisitLocalVariableDeclarator(LocalVariableDeclarator declarator) => DefaultVisit(declarator);
	public virtual TResult? VisitEqualsValueClause(EqualsValueClause clause) => DefaultVisit(clause);
	public virtual TResult? VisitTypeClause(TypeClauseSyntax clauseSyntax) => DefaultVisit(clauseSyntax);

	public virtual TResult? VisitEmptyStatement(EmptyStatementSyntax statement) => DefaultVisit(statement);
	public virtual TResult? VisitReturnStatement(ReturnStatementSyntax statement) => DefaultVisit(statement);
	public virtual TResult? VisitExpressionStatement(ExpressionStatementSyntax statement) => DefaultVisit(statement);
	public virtual TResult? VisitLocalVariableDeclarationStatement(LocalVariableDeclarationStatement statement) => DefaultVisit(statement);
	public virtual TResult? VisitBlockStatement(BlockStatementSyntax statement) => DefaultVisit(statement);
	public virtual TResult? VisitIfStatement(IfStatementSyntax statement) => DefaultVisit(statement);
	public virtual TResult? VisitLoopStatement(LoopStatementSyntax statement) => DefaultVisit(statement);
	public virtual TResult? VisitWhileStatement(WhileStatementSyntax statement) => DefaultVisit(statement);
	public virtual TResult? VisitBreakStatement(BreakStatementSyntax statement) => DefaultVisit(statement);
	public virtual TResult? VisitErrorStatement(ErrorStatementSyntax statement) => DefaultVisit(statement);
	public virtual TResult? VisitElseClause(ElseClauseSyntax elseClause) => DefaultVisit(elseClause);
	public virtual TResult? VisitLifetimeOrGenericConstraintClause(LifetimeOrGenericConstraintClauseSyntax elseClause) => DefaultVisit(elseClause);
	public virtual TResult? VisitAttribute(AttributeSyntax attribute) => DefaultVisit(attribute);
	public virtual TResult? VisitAttributeList(AttributeListSyntax attributeList) => DefaultVisit(attributeList);
	public virtual TResult? VisitAttributeDeclaration(AttributeDeclarationSyntax declaration) => DefaultVisit(declaration);
}