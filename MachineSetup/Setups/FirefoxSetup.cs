using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MachineSetup
{
    using static Global;

    [Setup("Firefox")]
    [SetupDependency(typeof(ChocolateySetup))]
    public class FirefoxSetup : ISetup
    {
        [SetupOption(Description = "The locale to install firefox in")]
        public string Locale = "en-US";

        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "firefox", $"-packageParameters \"l={Locale}\"");
        }
    }
}
