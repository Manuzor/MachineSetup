using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace MachineSetup
{
    using static Global;

    [Setup("Gimp")]
    [SetupDependency(typeof(ChocolateySetup))]
    public class GimpSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "gimp");
        }
    }
}
