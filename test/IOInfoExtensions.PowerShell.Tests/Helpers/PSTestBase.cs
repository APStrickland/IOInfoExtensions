using IOInfoExtensions.TestUtilities;
using System;
using System.IO;

namespace IOInfoExtensions.PowerShell.Tests.Helpers
{
    public class PSTestBase : TestBase
    {
        public readonly string modulePath;
        public readonly Version psVersion;

        public PSTestBase() : base()
        {
            modulePath = Path.Combine(testRootDirectory.Parent.FullName, "IOInfoExtensions.PowerShell", "IOInfoExtensions.PowerShell.psd1");
            #if PSV75
                psVersion = new Version(7, 5);
            #elif PSV74
                psVersion = new Version(7, 4);
            #elif PSV51
                psVersion = new Version(5, 1);
            #endif

            var version = PowerShellHelper.GetPowerShellVersion();
            if (!version.StartsWith($"{psVersion.Major}.{psVersion.Minor}"))
            {
                throw new Exception($"Using the wrong version of PowerShell. Expected {psVersion} but got {version}");
            }
        }
    }
}
