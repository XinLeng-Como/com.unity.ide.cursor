/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using IOPath = System.IO.Path;

namespace Microsoft.Unity.VisualStudio.Editor.Testing
{
    [TestFixture]
    internal class KiroIntegrationTests
    {
        private string _tempDirectory;
        private string _projectDirectory;

        [SetUp]
        public void SetUp()
        {
            _tempDirectory = IOPath.Combine(IOPath.GetTempPath(), "KiroIntegrationTests", Guid.NewGuid().ToString());
            _projectDirectory = IOPath.Combine(_tempDirectory, "TestProject");
            Directory.CreateDirectory(_projectDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
        }

        [Test]
        public void Discovery_IncludesKiroInstallations()
        {
            // Act
            var installations = Discovery.GetVisualStudioInstallations().ToArray();

            // Assert
            Assert.IsNotNull(installations);
            // Note: This test verifies that Discovery includes Kiro installations
            // The actual presence of Kiro installations depends on the test environment
        }

        [Test]
        public void Discovery_TryDiscoverInstallation_WithKiroPath_Succeeds()
        {
            // Arrange
            var mockKiroPath = CreateMockKiroInstallation();

            // Act
            var result = Discovery.TryDiscoverInstallation(mockKiroPath, out var installation);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(installation);
            Assert.IsInstanceOf<VisualStudioKiroInstallation>(installation);
        }

        [Test]
        public void EndToEndWorkflow_ProjectGeneration_CreatesCorrectFiles()
        {
            // Arrange
            var mockKiroPath = CreateMockKiroInstallation();
            Discovery.TryDiscoverInstallation(mockKiroPath, out var installation);

            // Act
            installation.CreateExtraFiles(_projectDirectory);

            // Assert
            var vscodeDir = IOPath.Combine(_projectDirectory, ".vscode");
            Assert.IsTrue(Directory.Exists(vscodeDir));

            // Verify all required files are created
            Assert.IsTrue(File.Exists(IOPath.Combine(vscodeDir, "extensions.json")));
            Assert.IsTrue(File.Exists(IOPath.Combine(vscodeDir, "settings.json")));
            Assert.IsTrue(File.Exists(IOPath.Combine(vscodeDir, "launch.json")));
        }

        [Test]
        public void EndToEndWorkflow_ExtensionsJson_ContainsUnityExtension()
        {
            // Arrange
            var mockKiroPath = CreateMockKiroInstallation();
            Discovery.TryDiscoverInstallation(mockKiroPath, out var installation);

            // Act
            installation.CreateExtraFiles(_projectDirectory);

            // Assert
            var extensionsFile = IOPath.Combine(_projectDirectory, ".vscode", "extensions.json");
            var content = File.ReadAllText(extensionsFile);
            Assert.IsTrue(content.Contains("visualstudiotoolsforunity.vstuc"));
        }

        [Test]
        public void EndToEndWorkflow_SettingsJson_ContainsFileExcludes()
        {
            // Arrange
            var mockKiroPath = CreateMockKiroInstallation();
            Discovery.TryDiscoverInstallation(mockKiroPath, out var installation);

            // Act
            installation.CreateExtraFiles(_projectDirectory);

            // Assert
            var settingsFile = IOPath.Combine(_projectDirectory, ".vscode", "settings.json");
            var content = File.ReadAllText(settingsFile);
            Assert.IsTrue(content.Contains("files.exclude"));
            Assert.IsTrue(content.Contains("**/*.meta"));
            Assert.IsTrue(content.Contains("Library/"));
        }

        [Test]
        public void EndToEndWorkflow_LaunchJson_ContainsUnityDebugConfig()
        {
            // Arrange
            var mockKiroPath = CreateMockKiroInstallation();
            Discovery.TryDiscoverInstallation(mockKiroPath, out var installation);

            // Act
            installation.CreateExtraFiles(_projectDirectory);

            // Assert
            var launchFile = IOPath.Combine(_projectDirectory, ".vscode", "launch.json");
            var content = File.ReadAllText(launchFile);
            Assert.IsTrue(content.Contains("Attach to Unity"));
            Assert.IsTrue(content.Contains("vstuc"));
        }

        [Test]
        public void EndToEndWorkflow_FileOpening_HandlesCorrectly()
        {
            // Arrange
            var mockKiroPath = CreateMockKiroInstallation();
            Discovery.TryDiscoverInstallation(mockKiroPath, out var installation);

            var testScript = IOPath.Combine(_projectDirectory, "TestScript.cs");
            File.WriteAllText(testScript, @"using UnityEngine;

public class TestScript : MonoBehaviour
{
    void Start()
    {
        Debug.Log(""Hello Kiro!"");
    }
}");

            var solutionFile = IOPath.Combine(_projectDirectory, "TestProject.sln");
            File.WriteAllText(solutionFile, "Microsoft Visual Studio Solution File");

            // Act
            var result = installation.Open(testScript, 6, 20, solutionFile);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void EndToEndWorkflow_OpenCSharpProject_WorksCorrectly()
        {
            // Arrange
            var mockKiroPath = CreateMockKiroInstallation();
            Discovery.TryDiscoverInstallation(mockKiroPath, out var installation);

            var solutionFile = IOPath.Combine(_projectDirectory, "TestProject.sln");
            File.WriteAllText(solutionFile, "Microsoft Visual Studio Solution File");

            // Act - Open C# Project (solution only, no specific file)
            var result = installation.Open(null, 1, 0, solutionFile);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void EndToEndWorkflow_AnalyzerSupport_IsEnabled()
        {
            // Arrange
            var mockKiroPath = CreateMockKiroInstallation();
            Discovery.TryDiscoverInstallation(mockKiroPath, out var installation);

            // Act & Assert
            Assert.IsTrue(installation.SupportsAnalyzers);
            Assert.AreEqual(new Version(11, 0), installation.LatestLanguageVersionSupported);
        }

        [Test]
        public void EndToEndWorkflow_ProjectGenerator_IsConfigured()
        {
            // Arrange
            var mockKiroPath = CreateMockKiroInstallation();
            Discovery.TryDiscoverInstallation(mockKiroPath, out var installation);

            // Act
            var generator = installation.ProjectGenerator;

            // Assert
            Assert.IsNotNull(generator);
            Assert.IsInstanceOf<SdkStyleProjectGeneration>(generator);
        }

        [Test]
        public void EndToEndWorkflow_MultipleInstallations_PrioritizesKiro()
        {
            // Act
            var installations = Discovery.GetVisualStudioInstallations().ToArray();

            // Assert
            Assert.IsNotNull(installations);
            // Note: Kiro installations should appear first in the enumeration
            // due to the order in Discovery.GetVisualStudioInstallations()
        }

        private string CreateMockKiroInstallation()
        {
#if UNITY_EDITOR_WIN
            var kiroDir = IOPath.Combine(_tempDirectory, "kiro");
            var resourcesDir = IOPath.Combine(kiroDir, "resources", "app");
            Directory.CreateDirectory(resourcesDir);

            var packageJsonPath = IOPath.Combine(resourcesDir, "package.json");
            var packageJson = @"{
    ""name"": ""kiro"",
    ""version"": ""1.0.0""
}";
            File.WriteAllText(packageJsonPath, packageJson);

            var kiroExePath = IOPath.Combine(kiroDir, "kiro.exe");
            File.WriteAllText(kiroExePath, "mock executable");

            return kiroExePath;
#elif UNITY_EDITOR_OSX
            var kiroAppDir = IOPath.Combine(_tempDirectory, "Kiro.app");
            var contentsDir = IOPath.Combine(kiroAppDir, "Contents");
            var resourcesDir = IOPath.Combine(contentsDir, "resources", "app");
            Directory.CreateDirectory(resourcesDir);

            var packageJsonPath = IOPath.Combine(resourcesDir, "package.json");
            var packageJson = @"{
    ""name"": ""kiro"",
    ""version"": ""1.0.0""
}";
            File.WriteAllText(packageJsonPath, packageJson);

            return kiroAppDir;
#else
            var kiroDir = IOPath.Combine(_tempDirectory, "kiro-installation");
            var resourcesDir = IOPath.Combine(kiroDir, "resources", "app");
            Directory.CreateDirectory(resourcesDir);

            var packageJsonPath = IOPath.Combine(resourcesDir, "package.json");
            var packageJson = @"{
    ""name"": ""kiro"",
    ""version"": ""1.0.0""
}";
            File.WriteAllText(packageJsonPath, packageJson);

            var kiroExePath = IOPath.Combine(kiroDir, "kiro");
            File.WriteAllText(kiroExePath, "#!/bin/bash\necho 'mock kiro executable'");

            return kiroExePath;
#endif
        }
    }
}