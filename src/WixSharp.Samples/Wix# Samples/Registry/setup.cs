//css_ref ..\..\WixSharp.dll;
//css_ref System.Core.dll;
using System;
using System.Linq;
using Microsoft.Win32;
using WixSharp;
using WixSharp.CommonTasks;

class Script
{
    static public void Main(string[] args)
    {
        //uncomment the line below if the reg file contains unsupported type to be ignored
        //RegFileImporter.SkipUnknownTypes = true;

        var fullSetup = new Feature("MyApp Binaries");

        Project project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(fullSetup, @"readme.txt")),
                new RegFile(fullSetup, "MyProduct.reg"), //RegFile does the same Tasks.ImportRegFil
                new RegValue(fullSetup, RegistryHive.LocalMachine, "Software\\My Company\\My Product", "Message", "Hello"),
                new RegValue(fullSetup, RegistryHive.LocalMachine, "Software\\My Company\\My Product", "Count", 777));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        project.UI = WUI.WixUI_ProgressOnly;

        Compiler.BuildMsi(project);
    }
}



