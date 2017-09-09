using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace MachineSetup
{
    using static Global;

    [Setup("Python 2.x")]
    [SetupDependency(typeof(ChocolateySetup))]
    public class Python2Setup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "python2");
        }
    }

    [Setup("Python 3.x", Links = new []{ @"https://docs.python.org/3/using/windows.html#installing-without-ui" })]
    public class Python3Setup : ISetup
    {
        [SetupOption(Description = "Compile all .py files to .pyc.")]
        public bool CompileAll = true;

        public void Run(SetupContext context)
        {
            int compileAllValue = CompileAll ? 1 : 0;
            context.ExecuteChocolatey("install", "python3", $"--install-arguments=CompileAll={compileAllValue}");
        }
    }
}
