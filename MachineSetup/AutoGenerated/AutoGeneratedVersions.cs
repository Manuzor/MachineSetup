using System;
using System.Text;
using System.Text.RegularExpressions;

namespace MachineSetup
{
    [Flags]
    public enum VersionFlags
    {
        Beta,
        RC1,
        RC2,
        RC3,
        Pre,
    }
}

