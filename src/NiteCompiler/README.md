# NiteCompiler Project

## Directory Structure
+ `CodeAnalysis/` — All related to the code analysis.
+ `CodeAnalysis/Syntax/` — Abstract syntax tree, lexing and parsing.
+ `CodeAnalysis/Syntax/Syntaxes/` — Syntax nodes, do not produce unique namespace.
+ `CodeAnalysis/Syntax/Text/` — Related to the process input text.
+ `CodeAnalysis/Declarations/` — Gathering type and module declarations.
+ `CodeAnalysis/Symbols/` — Symbols logic.
+ `CodeAnalysis/Binding/` — Binders, gather symbols from syntax and check for errors.
+ `Compilation/` — Mostly internal types related to the compilation process.
+ `Compiler/` — Function compiler.
+ `Dependencies/` — `.nlib` files processing logic.
+ `Diagnostics/` — Reporting logic, all diagnostics.
+ `IntermediateRepresentation/` — Intermediate representation logic.
+ `Emmiting/` — Translating libraries into binary formats.