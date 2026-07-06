# Nite Compiler `nitec` CLI

## Repository Structure
+ `src/NiteCompiler` — The Nite programming language compiler API.
+ `src/NiteCompiler.CliTool` — CLI interface over compiler.
+ `src/NiteCompiler.Tests` — Test project for compiler public and internal API.

---

> [!CAUTION]
> Compiler will be rewritten asap to the self-hosted Nite.

> [!WARN]
> The following is under development and can be (*definitely will be*) changed.

## Nlib format
Nlib format is used for packages:
```
file.nlib [zip format]
├── checksum.txt {unsure, is this really required}
├── lib-info.yml
├── lib-data.bin
├── src/ [optional]
│   ├── file1.nite
│   ├── file2.nite
│   └── file3.nite
└── native/ [optional]
    ├── x86_64-windows
    │   └── xyz.dll
    └── <any-target-triple>
        └── libxyz.so
```

### lib-info.yml
```yml
name: "library-name"
version: 1.0.0 # Valid sem-ver
author: [ "Author" ]
dependencies:
  - name: std
    rule: [ ">=1.0" ]
git: # Optional
  commit: 72dd14a8e478ba17ddaa6f962773d3dae9431fd2
  ref: https://github.com/NiTiS-Dev/std/
native-resolver: # Optional
  xyz: # Name map to actual file
    x86_64-windows: native/x86_64-windows/xyz.dll
```

### lib-data.bin

## Meta-attributes
Meta-attributes are used to provide specific information to compiler.

+ `#when PURE_EXPRESSION` member with this meta-attribute would be accessible only when expression is true.
+ `#compiler_impl` function with this attribute may not have body, as its implementation is delegated to compiler.
+ `#inline(never|always)` functions with this attribute would never|always be inlined.
+ `#export("export_name")` or `#export` functions and fields with this attribute is exported when compiler into native library.
You can override export symbol name by providing string argument. By default, name is used without names of containing.
+ `#import("")` or `#import("", "")`