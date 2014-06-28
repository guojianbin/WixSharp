//css_ref ..\..\WixSharp.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {
        Project project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"Files\2\MyApp.exe"),
					new File(@"Files\2\MyApp.cs"),
					new File(@"Files\2\manual.pdf")));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        project.Version = new Version("1.0.714.10040");
        project.MajorUpgradeStrategy = MajorUpgradeStrategy.Default;

        Compiler.BuildMsi(project, "setup.2.msi");
    }
}



