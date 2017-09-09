using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineSetup.Setups
{
    [Setup("Lock Hunter")]
    [SetupDependency(typeof(ChocolateySetup))]
    public class LockHunterSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "lockhunter");
        }
    }
}
