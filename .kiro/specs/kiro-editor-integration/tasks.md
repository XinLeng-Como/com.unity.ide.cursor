# Implementation Plan

- [x] 1. Update package metadata for Kiro integration





  - Modify package.json to change name from "com.boxqkrtm.ide.cursor" to "com.como.ide.kiro"
  - Update displayName from "Cursor Editor" to "Kiro Editor"
  - Update description to reference Kiro instead of Cursor
  - _Requirements: 6.1, 6.2, 6.4_

- [x] 2. Create VisualStudioKiroInstallation class


  - Create new file Editor/VisualStudioKiroInstallation.cs based on VisualStudioCursorInstallation.cs
  - Implement platform-specific Kiro detection logic for Windows, macOS, and Linux
  - Update executable name patterns to match Kiro instead of Cursor
  - _Requirements: 1.1, 1.2, 4.1, 4.2, 4.3_

- [x] 2.1 Implement Windows Kiro detection


  - Update Windows-specific paths to search for kiro.exe instead of cursor.exe
  - Modify registry detection logic if needed for Kiro installations
  - Update file existence checks and path validation for Windows
  - _Requirements: 1.1, 4.1_

- [x] 2.2 Implement macOS Kiro detection


  - Update macOS app bundle detection to search for Kiro*.app instead of Cursor*.app
  - Modify regex patterns to match Kiro application names
  - Update bundle structure parsing for Kiro package.json location
  - _Requirements: 1.1, 4.2_

- [x] 2.3 Implement Linux Kiro detection


  - Update Linux binary paths to search for "kiro" executable instead of "cursor"
  - Modify XDG desktop file parsing to look for kiro.desktop entries
  - Update process name detection for Linux Kiro processes
  - _Requirements: 1.1, 4.3_

- [x] 3. Update version detection and parsing


  - Modify package.json parsing logic to handle Kiro version formats
  - Update version comparison and prioritization logic
  - Ensure prerelease detection works with Kiro naming conventions
  - _Requirements: 1.2_

- [x] 4. Update process management for Kiro


  - Modify FindRunningKiroWithSolution method (rename from FindRunningCursorWithSolution)
  - Update process name detection to search for Kiro processes instead of Cursor
  - Modify command-line argument handling for Kiro-specific flags
  - _Requirements: 3.1, 3.2, 3.4_

- [x] 5. Update workspace configuration for Kiro compatibility


  - Modify .vscode folder creation to work with Kiro
  - Update extensions.json to recommend Kiro-compatible extensions
  - Ensure settings.json and launch.json work with Kiro's VS Code compatibility
  - _Requirements: 3.3, 2.1, 2.2, 2.3, 2.4_

- [x] 6. Update Discovery.cs to include Kiro installation


  - Add VisualStudioKiroInstallation.GetVisualStudioInstallations() to discovery enumeration
  - Add VisualStudioKiroInstallation.TryDiscoverInstallation() to discovery attempts
  - Add VisualStudioKiroInstallation.Initialize() to initialization sequence
  - _Requirements: 1.1, 1.3_

- [x] 7. Write unit tests for Kiro installation detection


  - Create test cases for Windows Kiro detection logic
  - Create test cases for macOS Kiro detection logic  
  - Create test cases for Linux Kiro detection logic
  - Test version parsing from Kiro package.json files
  - _Requirements: 1.1, 1.2, 4.1, 4.2, 4.3_

- [x] 8. Write unit tests for Kiro process management

  - Test Kiro process detection and window reuse functionality
  - Test file opening with line and column positioning
  - Test error handling for missing Kiro installations
  - Test workspace configuration file generation
  - _Requirements: 3.1, 3.2, 3.3, 3.4_

- [x] 9. Write integration tests for end-to-end workflow


  - Test complete workflow from Unity script double-click to Kiro opening
  - Test "Open C# Project" functionality with Kiro
  - Test project file generation and Kiro workspace setup
  - Test debugging integration compatibility
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 3.1, 3.2, 3.3, 5.1, 5.2, 5.3, 5.4_

- [x] 10. Update error handling and user messaging



  - Update error messages to reference Kiro instead of Cursor
  - Implement proper error handling for missing Kiro installations
  - Add user-friendly messages for Kiro detection failures
  - Test error scenarios across all supported platforms
  - _Requirements: 1.3, 3.4_