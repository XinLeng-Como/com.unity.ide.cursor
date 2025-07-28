// Simple test to verify interface compilation
using Microsoft.Unity.VisualStudio.Editor;

namespace Test
{
    public class CompilationTest
    {
        public void TestInterfaceProperties()
        {
            // This should compile without errors now
            IVisualStudioInstallation installation = null;
            if (installation != null)
            {
                var name = installation.Name;
                var path = installation.Path;
                var isPrerelease = installation.IsPrerelease;
                var supportsAnalyzers = installation.SupportsAnalyzers;
            }
        }
    }
}