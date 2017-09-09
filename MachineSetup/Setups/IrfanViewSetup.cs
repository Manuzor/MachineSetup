using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineSetup.Setups
{
    [Setup("IrfanView")]
    [SetupDependency(typeof(ChocolateySetup))]
    public class IrfanViewSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "irfanview");
        }
    }

    [Setup("IrfanView all plugins")]
    [SetupDependency(typeof(IrfanViewSetup))]
    public class IrfanViewAllPluginsSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "irfanviewplugins");
        }
    }
}
