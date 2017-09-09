using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MachineSetup
{
    using static Global;

    [Setup("Git")]
    [SetupDependency(typeof(ChocolateySetup))]
    public class GitSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            if(context.ExecuteChocolatey("install", "git.install") == 0)
            {
                // Write .gitconfig
                string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string gitconfigPath = Path.Combine(home, ".gitconfig");
                File.WriteAllText(gitconfigPath, Resources.gitconfig, Encoding.UTF8);
            }
        }
    }
}
