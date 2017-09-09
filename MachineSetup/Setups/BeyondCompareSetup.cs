using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

using HtmlAgilityPack;

namespace MachineSetup
{
    using static Global;

    [Setup("Beyond Compare 4")]
    [SetupDependency(typeof(ChocolateySetup))]
    public class BeyondCompareSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "beyondcompare");
        }
    }
}
