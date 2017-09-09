using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineSetup.Setups
{
    [Setup("KeePass 2.x")]
    [SetupDependency(typeof(ChocolateySetup))]
    public class KeePass2Setup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "keepass.install");
        }
    }
}
