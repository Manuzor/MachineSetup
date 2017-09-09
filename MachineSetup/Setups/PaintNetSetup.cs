using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using HtmlAgilityPack;

namespace MachineSetup
{
    using static Global;

    [Setup("Paint.NET")]
    [SetupDependency(typeof(ChocolateySetup))]
    public class PaintNetSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "paint.net");
        }
    }
}
