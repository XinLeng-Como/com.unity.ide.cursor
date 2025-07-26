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
    internal class VisualStudioKiroInstallationTests
    {
        private string _tempDirectory;

        [SetUp]
        public void SetUp()
        {
            _tempDirectory = IOPath.Combine(IOPath.GetTempPath(), "KiroInstallationTests", Guid.NewGuid().ToString());
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
        public void TryDiscoverInstallation_WithValidKiroPath_ReturnsTrue()
        {
            // Arrange
            var kiroPath = CreateMockKiroInstallation("1.0.0");

            // Act
            var result = VisualStudioKiroInstallation.TryDiscoverInstallation(kiroPath, out var installation);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(installation);
            Assert.AreEqual("Kiro [1.0.0]", installation.Name);
            Assert.AreEqual(kiroPath, installation.Path);
        }

        [Test]
        public void TryDiscoverInstallation_WithInvalidPath_ReturnsFalse()
        {
            // Act
            var result = VisualStudioKiroInstallation.TryDiscoverInstallation("/invalid/path", out var installation);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(installation);
        }

        [Test]
        public void TryDiscoverInstallation_WithEmptyPath_ReturnsFalse()
        {
            // Act
            var result = VisualStudioKiroInstallation.TryDiscoverInstallation("", out var installation);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(installation);
        }

        [Test]
        public void TryDiscoverInstallation_WithNullPath_ReturnsFalse()
        {
            // Act
            var result = VisualStudioKiroInstallation.TryDiscoverInstallation(null, out var installation);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(installation);
        }

        [Test]
        public void TryDiscoverInstallation_WithPrerelease_DetectsCorrectly()
        {
            // Arrange
            var kiroPath = CreateMockKiroInstallation("1.0.0-insider");

            // Act
            var result = VisualStudioKiroInstallation.TryDiscoverInstallation(kiroPath, out var installation);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(installation);
            Assert.IsTrue(installation.IsPrerelease);
            Assert.IsTrue(installation.Name.Contains("Insider"));
        }

        [Test]
        public void GetVisualStudioInstallations_ReturnsValidInstallations()
        {
            // Act
            var installations = VisualStudioKiroInstallation.GetVisualStudioInstallations().ToArray();

            // Assert
            Assert.IsNotNull(installations);
            // Note: This test may return empty results if Kiro is not installed on the test machine
            // but it should not throw exceptions
        }

        private string CreateMockKiroInstallation(string version)
        {
#if UNITY_EDITOR_WIN
            return CreateMockWindowsKiroInstallation(version);
#elif UNITY_EDITOR_OSX
            return CreateMockMacOSKiroInstallation(version);
#else
            return CreateMockLinuxKiroInstallation(version);
#endif
        }

#if UNITY_EDITOR_WIN
        private string CreateMockWindowsKiroInstallation(string version)
        {
            var kiroDir = IOPath.Combine(_tempDirectory, "kiro");
            var resourcesDir = IOPath.Combine(kiroDir, "resources", "app");
            Directory.CreateDirectory(resourcesDir);

            var packageJsonPath = IOPath.Combine(resourcesDir, "package.json");
            var packageJson = $@"{{
    ""name"": ""kiro"",
    ""version"": ""{version}""
}}";
            File.WriteAllText(packageJsonPath, packageJson);

            var kiroExePath = IOPath.Combine(kiroDir, "kiro.exe");
            File.WriteAllText(kiroExePath, "mock executable");

            return kiroExePath;
        }
#endif

#if UNITY_EDITOR_OSX
        private string CreateMockMacOSKiroInstallation(string version)
        {
            var kiroAppDir = IOPath.Combine(_tempDirectory, "Kiro.app");
            var contentsDir = IOPath.Combine(kiroAppDir, "Contents");
            var resourcesDir = IOPath.Combine(contentsDir, "resources", "app");
            Directory.CreateDirectory(resourcesDir);

            var packageJsonPath = IOPath.Combine(resourcesDir, "package.json");
            var packageJson = $@"{{
    ""name"": ""kiro"",
    ""version"": ""{version}""
}}";
            File.WriteAllText(packageJsonPath, packageJson);

            return kiroAppDir;
        }
#endif

#if UNITY_EDITOR_LINUX
        private string CreateMockLinuxKiroInstallation(string version)
        {
            var kiroDir = IOPath.Combine(_tempDirectory, "kiro-installation");
            var resourcesDir = IOPath.Combine(kiroDir, "resources", "app");
            Directory.CreateDirectory(resourcesDir);

            var packageJsonPath = IOPath.Combine(resourcesDir, "package.json");
            var packageJson = $@"{{
    ""name"": ""kiro"",
    ""version"": ""{version}""
}}";
            File.WriteAllText(packageJsonPath, packageJson);

            var kiroExePath = IOPath.Combine(kiroDir, "kiro");
            File.WriteAllText(kiroExePath, "#!/bin/bash\necho 'mock kiro executable'");

            return kiroExePath;
        }
#endif
    }
}