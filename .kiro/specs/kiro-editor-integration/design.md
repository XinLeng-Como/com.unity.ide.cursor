# Design Document

## Overview

This design outlines the adaptation of the existing Unity Cursor editor integration package to work with Kiro. The current package provides seamless integration between Unity and Cursor through auto-discovery, project file generation, and workspace configuration. We will modify the core components to detect and integrate with Kiro installations while maintaining the same architecture and functionality patterns.

The adaptation involves creating a new `VisualStudioKiroInstallation` class that follows the same patterns as the existing `VisualStudioCursorInstallation`, updating the discovery mechanism, and modifying package metadata to reflect the Kiro integration.

## Architecture

The package follows Unity's `IExternalCodeEditor` plugin architecture with these key components:

### Core Components
- **VisualStudioEditor**: Main entry point implementing `IExternalCodeEditor`
- **Discovery**: Central discovery mechanism for all supported editors
- **VisualStudioKiroInstallation**: New Kiro-specific installation handler
- **Project Generation**: Existing SDK-style project generation system
- **Platform Integration**: Cross-platform installation detection

### Integration Flow
1. Unity loads the package and registers `VisualStudioEditor` with `CodeEditor`
2. Discovery system scans for Kiro installations across platforms
3. When user opens files, the system routes through Kiro installation handler
4. Project files are generated and workspace is configured for Kiro

## Components and Interfaces

### New Components

#### VisualStudioKiroInstallation
```csharp
internal class VisualStudioKiroInstallation : VisualStudioInstallation
{
    // Platform-specific Kiro detection logic
    // Workspace configuration for Kiro
    // Process management for opening files
}
```

**Responsibilities:**
- Detect Kiro installations on Windows, macOS, and Linux
- Parse Kiro version information from package.json
- Configure .vscode workspace settings for Kiro compatibility
- Handle file opening with proper line/column positioning
- Manage Kiro process lifecycle and window reuse

#### Platform-Specific Detection Logic
- **Windows**: Search in `%LOCALAPPDATA%\Programs\kiro` and `%PROGRAMFILES%\kiro`
- **macOS**: Search in `/Applications` for `Kiro*.app` bundles
- **Linux**: Search standard binary paths and XDG desktop entries

### Modified Components

#### Discovery.cs
Update to include Kiro installation discovery:
```csharp
public static IEnumerable<IVisualStudioInstallation> GetVisualStudioInstallations()
{
    foreach (var installation in VisualStudioKiroInstallation.GetVisualStudioInstallations())
        yield return installation;
    // ... existing installations
}
```

#### package.json
Update package metadata:
- Name: `com.como.ide.kiro`
- Display Name: "Kiro Editor"
- Description: Updated to reference Kiro instead of Cursor

### Existing Components (Unchanged)
- **VisualStudioEditor**: Core integration logic remains the same
- **Project Generation**: SDK-style project generation works with any VS Code-compatible editor
- **File Utilities**: Cross-platform file operations remain unchanged

## Data Models

### Installation Detection Model
```csharp
public class KiroInstallationInfo
{
    public string Path { get; set; }
    public Version Version { get; set; }
    public bool IsPrerelease { get; set; }
    public string Name { get; set; }
}
```

### Workspace Configuration Model
The existing `.vscode` workspace configuration will be reused since Kiro is VS Code-compatible:
- `settings.json`: File exclusions and default solution
- `launch.json`: Debugging configuration for VSTU
- `extensions.json`: Recommended extensions (updated for Kiro-compatible extensions)

## Error Handling

### Installation Detection Errors
- **No Installation Found**: Display user-friendly message directing to Kiro installation
- **Multiple Installations**: Prioritize latest version, allow user selection in preferences
- **Corrupted Installation**: Graceful fallback with warning message
- **Permission Issues**: Handle access denied scenarios on Linux/macOS

### File Opening Errors
- **Kiro Not Running**: Auto-launch Kiro with appropriate workspace
- **File Not Found**: Validate file existence before opening
- **Process Errors**: Retry mechanism for process communication failures

### Project Generation Errors
- **Write Permission Issues**: Clear error messages for read-only directories
- **Malformed Project Files**: Validation and recovery mechanisms
- **Missing Dependencies**: Graceful handling of missing Unity packages

## Testing Strategy

### Unit Tests
- **Installation Discovery**: Mock file system to test detection logic across platforms
- **Version Parsing**: Test version extraction from various Kiro package.json formats
- **Path Normalization**: Cross-platform path handling validation
- **Process Management**: Mock process execution for file opening tests

### Integration Tests
- **End-to-End Workflow**: Full integration from Unity to Kiro file opening
- **Project Generation**: Validate generated .csproj and .sln files
- **Workspace Configuration**: Verify .vscode folder creation and content
- **Multi-Platform**: Test installation detection on Windows, macOS, and Linux

### Manual Testing Scenarios
- Install Kiro in various locations and verify detection
- Test file opening with different line/column positions
- Verify project regeneration after Unity package changes
- Test debugging integration with VSTU
- Validate workspace configuration in Kiro

### Performance Considerations
- **Async Discovery**: Installation discovery runs asynchronously to avoid Unity startup delays
- **Cached Results**: Cache discovered installations to avoid repeated file system scans
- **Process Reuse**: Reuse existing Kiro windows when possible to improve performance

### Security Considerations
- **Path Validation**: Sanitize all file paths to prevent directory traversal
- **Process Execution**: Validate executable paths before launching processes
- **File Permissions**: Respect file system permissions when creating workspace files

## Platform-Specific Implementation Details

### Windows Implementation
- Use registry queries as fallback for installation detection
- Handle Windows path separators and drive letters
- Support both user and system-wide installations

### macOS Implementation
- Handle .app bundle structure for version detection
- Use `open` command for proper application launching
- Support both Applications and user-specific installations

### Linux Implementation
- Parse XDG desktop files for installation paths
- Handle symbolic links using readlink syscall
- Support various distribution-specific installation paths

## Migration Strategy

### From Cursor to Kiro
- Maintain backward compatibility during transition
- Provide clear migration documentation
- Support side-by-side installation during testing

### Version Management
- Follow semantic versioning for package updates
- Maintain changelog for user-facing changes
- Handle Unity version compatibility requirements