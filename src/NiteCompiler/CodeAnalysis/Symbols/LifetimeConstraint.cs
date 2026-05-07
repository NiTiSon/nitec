namespace NiteCompiler.CodeAnalysis.Symbols;

public readonly record struct LifetimeConstraint(LifetimeSymbol Longer, LifetimeSymbol Shorter);