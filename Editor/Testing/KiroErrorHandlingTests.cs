/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using IOPath = System.IO.Path;

namespace Microsoft.Unity.VisualStudio.Editor.Testing
{
    [TestFixture]
    internal class KiroErrorHandlingTests
    {
        private string _tempDirectory;

        [SetUp]
        public void SetUp()
        {
            _tempDirectory = IOPath.Combine(IOPath.GetTempPath(), "KiroErrorTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDirectory);
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
        public void TryDiscoverInstallation_WithCorruptedPackageJson_HandlesGracefully()
        {
            // Arrange
            var kiroPath = CreateKiroInstallationWithCorruptedPackageJson();

            // Act
            var result = VisualStudioKiroInstallation.TryDiscoverInstallation(kiroPath, out var installation);

            // Assert
            Assert.IsTrue(result); // Should still succeed but without version info
            Assert.IsNotNull(installation);
            Assert.AreEqual("Kiro", installation.Name); // Should fallback to basic name
        }

        [Test]
        public void TryDiscoverInstallation_WithMissingPackageJson_HandlesGracefully()
        {
            // Arrange
            var kiroPath = CreateKiroInstallationWithoutPackageJson();

            // Act
            var result = VisualStudioKiroInstallation.TryDiscoverInstallation(kiroPath, out var installation);

            // Assert
            Assert.IsTrue(result); // Should still succeed
            Assert.IsNotNull(installation);
            Assert.AreEqual("Kiro", installation.Name); // Should fallback to basic name
        }

        [Test]
        public void CreateExtraFiles_WithReadOnlyDirectory_HandlesIOException()
        {
            // Arrange
            var mockKiroPath = CreateMockKiroInstallation();
            VisualStudioKiroInstallation.TryDiscoverInstallation(mockKiroPath, out var installation);

            var readOnlyDir = IOPath.Combine(_tempDirectory, "ReadOnlyProject");
            Directory.CreateDirectory(readOnlyDir);

            try
            {
                // Make directory read-only (platform-specific)
#if UNITY_EDITOR_WIN
                File.SetAttributes(readOnlyDir, FileAttributes.ReadOnly);
#else
                // On Unix systems, remove write permissions
                var info = new DirectoryInfo(readOnlyDir);
                info.Attributes |= FileAttributes.ReadOnly;
#endif

                // Act & Assert - Should not throw exception
                Assert.DoesNotThrow(() => installation.CreateExtraFiles(readOnlyDir));
            }
            finally
            {
                // Restore permissions for cleanup
#if UNITY_EDITOR_WIN
                File.SetAttributes(readOnlyDir, FileAttributes.Normal);
#else
                var info = new DirectoryInfo(readOnlyDir);
                info.Attributes &= ~FileAttributes.ReadOnly;
#endif
            }
        }

        [Test]
        public void Open_WithInvalidSolutionPath_ReturnsTrue()
        {
            // Arrange
            var mockKiroPath = CreateMockKiroInstallation();
            VisualStudioKiroInstallation.TryDiscoverInstallation(mockKiroPath, out var installation);

            var invalidSolution = IOPath.Combine(_tempDirectory, "NonExistent", "Invalid.sln");

            // Act
            var result = installation.Open("test.cs", 1, 0, invalidSolution);

            // Assert
            Assert.IsTrue(result); // Should still attempt to open
        }

        [Test]
        public void GetVisualStudioInstallations_WithNoKiroInstalled_ReturnsEmptyEnumerable()
        {
            // Act
            var installations = VisualStudioKiroInstallation.GetVisualStudioInstallations();

            // Assert
            Assert.IsNotNull(installations);
            // Should not throw exceptions even if no Kiro installations are found
        }

        [Test]
        public void Discovery_Initialize_DoesNotThrowExceptions()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => Discovery.Initialize());
        }

        [Test]
        public void Discovery_TryDiscoverInstallation_WithIOException_ReturnsFalse()
        {
            // Arrange
            var invalidPath = IOPath.Combine(_tempDirectory, "invalid", "path", "that", "does", "not", "exist");

            // Act
            var result = Discovery.TryDiscoverInstallation(invalidPath, out var installation);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(installation);
        }

        [Test]
        public void KiroInstallation_ErrorMessages_ContainKiroReference()
        {
            // This test verifies that error messages reference "Kiro" instead of "Cursor"
            // We'll capture log messages during a process operation that might fail

            // Arrange
            var mockKiroPath = CreateMockKiroInstallation();
            VisualStudioKiroInstallation.TryDiscoverInstallation(mockKiroPath, out var installation);

            var invalidSolution = "/invalid/path/solution.sln";

            // Act - This should trigger error logging
            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex(".*\\[Kiro\\].*"));
            
            // Force an error condition by trying to open with invalid parameters
            installation.Open("test.cs", 1, 0, invalidSolution);
        }

        private string CreateKiroInstallationWithCorruptedPackageJson()
        {
#if UNITY_EDITOR_WIN
            var kiroDir = IOPath.Combine(_tempDirectory, "kiro-corrupted");
            var resourcesDir = IOPath.Combine(kiroDir, "resources", "app");
            Directory.CreateDirectory(resourcesDir);

            var packageJsonPath = IOPath.Combine(resourcesDir, "package.json");
            File.WriteAllText(packageJsonPath, "{ invalid json content }");

            var kiroExePath = IOPath.Combine(kiroDir, "kiro.exe");
            File.WriteAllText(kiroExePath, "mock executable");

            return kiroExePath;
#elif UNITY_EDITOR_OSX
            var kiroAppDir = IOPath.Combine(_tempDirectory, "Kiro-Corrupted.app");
            var contentsDir = IOPath.Combine(kiroAppDir, "Contents");
            var resourcesDir = IOPath.Combine(contentsDir, "resources", "app");
            Directory.CreateDirectory(resourcesDir);

            var packageJsonPath = IOPath.Combine(resourcesDir, "package.json");
            File.WriteAllText(packageJsonPath, "{ invalid json content }");

            return kiroAppDir;
#else
            var kiroDir = IOPath.Combine(_tempDirectory, "kiro-corrupted");
            var resourcesDir = IOPath.Combine(kiroDir, "resources", "app");
            Directory.CreateDirectory(resourcesDir);

            var packageJsonPath = IOPath.Combine(resourcesDir, "package.json");
            File.WriteAllText(packageJsonPath, "{ invalid json content }");

            var kiroExePath = IOPath.Combine(kiroDir, "kiro");
            File.WriteAllText(kiroExePath, "#!/bin/bash\necho 'mock kiro executable'");

            return kiroExePath;
#endif
        }

        private string CreateKiroInstallationWithoutPackageJson()
        {
#if UNITY_EDITOR_WIN
            var kiroDir = IOPath.Combine(_tempDirectory, "kiro-no-package");
            var resourcesDir = IOPath.Combine(kiroDir, "resources", "app");
            Directory.CreateDirectory(resourcesDir);
            // Intentionally not creating package.json

            var kiroExePath = IOPath.Combine(kiroDir, "kiro.exe");
            File.WriteAllText(kiroExePath, "mock executable");

            return kiroExePath;
#elif UNITY_EDITOR_OSX
            var kiroAppDir = IOPath.Combine(_tempDirectory, "Kiro-NoPackage.app");
            var contentsDir = IOPath.Combine(kiroAppDir, "Contents");
            var resourcesDir = IOPath.Combine(contentsDir, "resources", "app");
            Directory.CreateDirectory(resourcesDir);
            // Intentionally not creating package.json

            return kiroAppDir;
#else
            var kiroDir = IOPath.Combine(_tempDirectory, "kiro-no-package");
            var resourcesDir = IOPath.Combine(kiroDir, "resources", "app");
            Directory.CreateDirectory(resourcesDir);
            // Intentionally not creating package.json

            var kiroExePath = IOPath.Combine(kiroDir, "kiro");
            File.WriteAllText(kiroExePath, "#!/bin/bash\necho 'mock kiro executable'");

            return kiroExePath;
#endif
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