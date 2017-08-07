using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MachineSetup
{
    public static class Metadata
    {
        public static readonly Assembly This = Assembly.GetAssembly(typeof(Metadata));
        public static string Name => This.GetName().Name;
        public static Version Version => This.GetName().Version;
    }
}
