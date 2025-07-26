# Technology Stack

## Core Technologies
- **Language**: C# (.NET Framework compatible with Unity)
- **Platform**: Unity Editor Package (Unity Package Manager)
- **Target Framework**: .NET Standard 2.1
- **Minimum Unity Version**: 2019.4.25f1

## Key Dependencies
- Unity Editor APIs (`UnityEditor`, `UnityEngine`)
- Unity Code Editor API (`Unity.CodeEditor`)
- Newtonsoft.Json.dll (precompiled reference)
- Unity Test Framework (1.1.9) for testing

## Architecture Patterns
- **Plugin Architecture**: Implements Unity's `IExternalCodeEditor` interface
- **Discovery Pattern**: Auto-detection of editor installations across platforms
- **Factory Pattern**: Installation discovery and creation
- **Observer Pattern**: File change monitoring and project regeneration
- **Command Pattern**: CLI integration and process execution

## Platform-Specific Code
- Conditional compilation using `#if UNITY_EDITOR_WIN`, `#if UNITY_EDITOR_OSX`, `#if UNITY_EDITOR_LINUX`
- Platform-specific installation discovery logic
- Native interop for Linux (readlink syscall)

## Build System
- **Package Format**: Unity Package Manager (UPM) package
- **Assembly Definition**: `Unity.Kiro.Editor.asmdef`
- **Editor-Only**: Package only runs in Unity Editor, not in builds

## Common Commands
Since this is a Unity package, there are no traditional build commands. The package is:
- Installed via Unity Package Manager
- Automatically compiled by Unity when imported
- Tested through Unity's Test Runner framework

## Development Workflow
- Code changes are automatically compiled by Unity
- Testing done through Unity Test Runner
- Package distribution via Git URL or local package
- Version management through `package.json`