# Nite Compiler `nitec` CLI

## Nlib format
Nlib format is used for packages:
```
file.nlib [zip format]
├── checksum.txt {unsure, is this really required}
├── lib-info.yml
├── lib-data.bin
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