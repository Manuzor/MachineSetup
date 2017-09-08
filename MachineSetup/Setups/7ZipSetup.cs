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

    public class SevenZipSetup
    {
        public string FMRegistryKey = @"HKEY_CURRENT_USER\Software\7-Zip\FM";
        public string EditorPath = "C:/Program Files/Sublime Text 3/subl.exe";

        public void Run(SetupContext context)
        {
            if(ExecuteChocolatey(context, ChocoExePath, "install", "7zip.install") == 0)
            {
                Registry.SetValue(FMRegistryKey, "Editor", EditorPath);
            }
        }
    }
}
