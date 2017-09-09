using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MachineSetup
{
    using static Global;

    [Setup("Visual Studio 2017 Community")]
    [SetupDependency(typeof(ChocolateySetup))]
    public class VisualStudio2017CommunitySetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "visualstudio2017community");
        }
    }
}
