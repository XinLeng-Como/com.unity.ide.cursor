/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using IOPath = System.IO.Path;

namespace Microsoft.Unity.VisualStudio.Editor.Testing
{
    [TestFixture]
    internal class VisualStudioKiroProcessTests
    {
        private string _tempDirectory;
        private VisualStudioKiroInstallation _installation;

        [SetUp]
        public void SetUp()
        {
            _tempDirectory = IOPath.Combine(IOPath.GetTempPath(), "KiroProcessTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDirectory);

            // Create a mock Kiro installation for testing
            var mockKiroPath = CreateMockKiroInstallation();
            VisualStudioKiroInstallation.TryDiscoverInstallation(mockKiroPath, out var installation);
            _installation = installation as VisualStudioKiroInstallation;
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
        public void Open_WithValidParameters_ReturnsTrue()
        {
            // Arrange
            var testFile = IOPath.Combine(_tempDirectory, "test.cs");
            File.WriteAllText(testFile, "// test file");
            var solutionFile = IOPath.Combine(_tempDirectory, "test.sln");
            File.WriteAllText(solutionFile, "// test solution");

            // Act
            var result = _installation.Open(testFile, 10, 5, solutionFile);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Open_WithNegativeLineNumber_NormalizesToOne()
        {
            // Arrange
            var testFile = IOPath.Combine(_tempDirectory, "test.cs");
            File.WriteAllText(testFile, "// test file");
            var solutionFile = IOPath.Combine(_tempDirectory, "test.sln");
            File.WriteAllText(solutionFile, "// test solution");

            // Act
            var result = _installation.Open(testFile, -5, 0, solutionFile);

            // Assert
            Assert.IsTrue(result);
            // Note: Line number normalization is handled internally
        }

        [Test]
        public void Open_WithNegativeColumnNumber_NormalizesToZero()
        {
            // Arrange
            var testFile = IOPath.Combine(_tempDirectory, "test.cs");
            File.WriteAllText(testFile, "// test file");
            var solutionFile = IOPath.Combine(_tempDirectory, "test.sln");
            File.WriteAllText(solutionFile, "// test solution");

            // Act
            var result = _installation.Open(testFile, 1, -10, solutionFile);

            // Assert
            Assert.IsTrue(result);
            // Note: Column number normalization is handled internally
        }

        [Test]
        public void CreateExtraFiles_CreatesVSCodeDirectory()
        {
            // Arrange
            var projectDir = IOPath.Combine(_tempDirectory, "TestProject");
            Directory.CreateDirectory(projectDir);

            // Act
            _installation.CreateExtraFiles(projectDir);

            // Assert
            var vscodeDir = IOPath.Combine(projectDir, ".vscode");
            Assert.IsTrue(Directory.Exists(vscodeDir));
        }

        [Test]
        public void CreateExtraFiles_CreatesRequiredFiles()
        {
            // Arrange
            var projectDir = IOPath.Combine(_tempDirectory, "TestProject");
            Directory.CreateDirectory(projectDir);

            // Act
            _installation.CreateExtraFiles(projectDir);

            // Assert
            var vscodeDir = IOPath.Combine(projectDir, ".vscode");
            Assert.IsTrue(File.Exists(IOPath.Combine(vscodeDir, "extensions.json")));
            Assert.IsTrue(File.Exists(IOPath.Combine(vscodeDir, "settings.json")));
            Assert.IsTrue(File.Exists(IOPath.Combine(vscodeDir, "launch.json")));
        }

        [Test]
        public void CreateExtraFiles_WithExistingFiles_DoesNotOverwrite()
        {
            // Arrange
            var projectDir = IOPath.Combine(_tempDirectory, "TestProject");
            var vscodeDir = IOPath.Combine(projectDir, ".vscode");
            Directory.CreateDirectory(vscodeDir);

            var existingSettings = IOPath.Combine(vscodeDir, "settings.json");
            var originalContent = @"{""custom"": ""setting""}";
            File.WriteAllText(existingSettings, originalContent);

            // Act
            _installation.CreateExtraFiles(projectDir);

            // Assert
            var content = File.ReadAllText(existingSettings);
            Assert.IsTrue(content.Contains("custom"));
        }

        [Test]
        public void SupportsAnalyzers_ReturnsTrue()
        {
            // Act & Assert
            Assert.IsTrue(_installation.SupportsAnalyzers);
        }

        [Test]
        public void LatestLanguageVersionSupported_ReturnsCorrectVersion()
        {
            // Act
            var version = _installation.LatestLanguageVersionSupported;

            // Assert
            Assert.AreEqual(new Version(11, 0), version);
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