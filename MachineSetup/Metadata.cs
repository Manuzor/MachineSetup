using System;
using System.Reflection;

namespace MachineSetup
{
    public static partial class Global
    {
        public static readonly Assembly ThisAssembly = Assembly.GetAssembly(typeof(Global));
        public static string GlobalName => ThisAssembly.GetName().Name;
        public static Version GlobalVersion => ThisAssembly.GetName().Version;
    }
}
