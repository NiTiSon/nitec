namespace Nlr.Compiler.NiteCode.CodeAnalysis;

public interface IOperation
{
	SemanticModel SemanticModel { get; }
	
	OperationKind Kind { get; }
}