# Kiro Editor Integration for Unity

This Unity package provides seamless integration between Unity and the Kiro code editor, enabling developers to use Kiro as their primary code editor for Unity projects.

## Features

- **Auto-discovery** of Kiro installations across platforms (Windows, macOS, Linux)
- **C# project file generation** (.csproj) for IntelliSense support
- **Solution file management** for proper project structure
- **Debugging integration** through Visual Studio Tools for Unity (VSTU)
- **Workspace configuration** with automatic .vscode folder setup
- **Cross-platform support** with platform-specific installation detection

## How to install
- Unity -> Window -> Package Manager  
- Click "+" at the top left corner  
- Add package from git URL  
- Insert `https://github.com/XinLeng-Como/unity-kiro-integration.git`  
- Add  
- Done

## Requirements

- **Unity Version**: 2019.4.25f1 or later
- **Kiro Editor**: Any version compatible with VS Code extensions
- **.NET Standard**: 2.1 support

## Usage

1. **Install the package** using the instructions above
2. **Install Kiro** on your system if not already installed
3. **Set Kiro as external editor**:
   - Go to Unity -> Preferences -> External Tools
   - Set "External Script Editor" to your Kiro installation
   - Unity will auto-detect Kiro installations in standard locations

4. **Open scripts**: Double-click any C# script in Unity to open it in Kiro
5. **Open C# Project**: Use Assets -> Open C# Project to open the entire solution in Kiro

## Platform Support

### Windows
- Searches in `%LOCALAPPDATA%\Programs\kiro\kiro.exe`
- Searches in `%PROGRAMFILES%\kiro\kiro.exe`
- Supports both stable and insider versions

### macOS
- Searches in `/Applications/Kiro*.app`
- Supports app bundle detection with proper version parsing

### Linux
- Searches in standard binary locations (`/usr/bin/kiro`, `/bin/kiro`, `/usr/local/bin/kiro`)
- Supports XDG desktop file detection (`kiro.desktop`)
- Compatible with various Linux distributions

## Workspace Configuration

The package automatically creates a `.vscode` folder in your Unity project with:

- **extensions.json**: Recommends Unity-compatible VS Code extensions
- **settings.json**: Configures file exclusions and solution settings
- **launch.json**: Sets up Unity debugging configuration

## Troubleshooting

### Kiro Not Detected
- Ensure Kiro is installed in a standard location
- Check Unity -> Preferences -> External Tools for manual path setting
- Verify Kiro executable permissions (Linux/macOS)

### IntelliSense Issues
- Ensure the Unity project has been opened as a folder in Kiro
- Check that .csproj files are generated in your project
- Verify that the Visual Studio Tools for Unity extension is installed

### Debugging Issues
- Ensure the Unity debugging extension is installed in Kiro
- Check that launch.json is properly configured in the .vscode folder
- Verify Unity is running when attempting to attach debugger

## Version History

> **Important Notice for Users Updating from Cursor Integration**  
> Starting from version **v2.0.25**, this package has been updated to support Kiro instead of Cursor.  
> The package name has been changed from `com.boxqkrtm.ide.cursor` to `com.como.ide.kiro`.  
> If you were previously using the Cursor integration, please remove the old package before installing this one to avoid conflicts.

## Contributing

This package is based on Unity's Visual Studio Code integration and has been adapted for Kiro compatibility. 

## License

This project is licensed under the MIT License - see the LICENSE.md file for details.

## Support

For issues related to:
- **Unity integration**: Create an issue in this repository
- **Kiro editor**: Contact Kiro support
- **VS Code extensions**: Check the respective extension documentation
