using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MachineSetup
{
    using static Global;

    [Setup("NextCloud client")]
    [SetupDependency(typeof(ChocolateySetup))]
    public class NextCloudSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "nextcloud-client");
        }
    }
}
