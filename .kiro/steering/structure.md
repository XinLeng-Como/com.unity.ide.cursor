# Project Structure

## Root Level
- `package.json` - Unity package manifest with metadata and dependencies
- `README.md` - Installation instructions and package information
- `CHANGELOG.md` - Version history and changes
- `CONTRIBUTING.md` - Contribution guidelines and license agreements
- `LICENSE.md` - MIT license
- `ThirdPartyNotices.md` - Third-party license notices
- `ValidationConfig.json` - Unity package validation configuration
- `ValidationExceptions.json` - Validation rule exceptions

## Core Implementation (`Editor/`)
All source code is in the `Editor/` folder since this is an editor-only package.

### Main Classes
- `VisualStudioEditor.cs` - Main entry point, implements `IExternalCodeEditor`
- `VisualStudioKiroInstallation.cs` - Kiro-specific installation handling
- `VisualStudioCursorInstallation.cs` - Cursor-specific installation handling (legacy)
- `VisualStudioIntegration.cs` - Unity integration logic
- `Discovery.cs` - Editor installation discovery
- `Cli.cs` - Command-line interface support

### Project Generation (`Editor/ProjectGeneration/`)
- Handles .csproj and .sln file generation
- SDK-style project generation support
- Assembly and package reference management

### Platform Support
- `VisualStudioInstallation.cs` - Base installation class
- `VisualStudioCodiumInstallation.cs` - VSCodium support
- Platform-specific discovery logic embedded in installation classes

### Utilities
- `FileUtility.cs` - File system operations
- `ProcessRunner.cs` - External process execution
- `SimpleJSON.cs` - JSON parsing utilities
- `Solution.cs`, `SolutionParser.cs` - Solution file handling
- `UsageUtility.cs` - Usage analytics
- `UnityInstallation.cs` - Unity installation detection

### Testing (`Editor/Testing/`)
- Unit tests for the package functionality
- Integration tests for editor discovery

### Platform Integration
- `Editor/COMIntegration/` - Windows COM integration
- `Editor/VSWhere/` - Visual Studio detection on Windows
- `Editor/Messaging/` - Communication with external editors
- `Editor/Plugins/` - Platform-specific binaries

## File Naming Conventions
- All C# files use PascalCase
- `.meta` files accompany each asset (Unity requirement)
- Assembly definition: `com.unity.ide.visualstudio.asmdef` (contains `Unity.Kiro.Editor` assembly)

## Documentation (`Documentation~/`)
- `README.md` - Package documentation
- `index.md` - Documentation index
- `TableOfContents.md` - Documentation structure
- `using-visual-studio-editor.md` - Usage guide
- `Images/` - Documentation assets

## Key Architecture Notes
- Single assembly (`Unity.Kiro.Editor`) for all functionality
- Editor-only code (no runtime components)
- Platform-specific code using conditional compilation
- Follows Unity package structure conventions
- All public APIs are in `Microsoft.Unity.VisualStudio.Editor` namespace