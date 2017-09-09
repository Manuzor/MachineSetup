using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace MachineSetup
{
    using static Global;

    [Setup("7-Zip")]
    [SetupDependency(typeof(ChocolateySetup))]
    public class SevenZipSetup : ISetup
    {
        [SetupOption("Windows Registry Key 'FM'")]
        public string FMRegistryKey = @"HKEY_CURRENT_USER\Software\7-Zip\FM";

        [SetupOption("Editor Path", Description = "Path to the editor used by the 7-Zip file manager.")]
        public string EditorPath = "C:/Program Files/Sublime Text 3/subl.exe";

        public void Run(SetupContext context)
        {
            if(context.ExecuteChocolatey("install", "7zip.install") == 0)
            {
                Registry.SetValue(FMRegistryKey, "Editor", EditorPath);
            }
        }
    }
}
