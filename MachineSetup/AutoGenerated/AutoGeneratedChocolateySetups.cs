
namespace MachineSetup
{
    [Setup("Blender")]
    public class BlenderSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "blender");
        }
    }

    [Setup("Lock Hunter", Description = "(Shell integration) Discover which process is locking a file.")]
    public class LockHunterSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "lockhunter");
        }
    }

    [Setup("KeePass 2.x")]
    public class KeePassSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "keepass.install");
        }
    }

    [Setup("NextCloud client")]
    public class NextCloudSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "nextcloud-client");
        }
    }

    [Setup("Paint.NET")]
    public class PaintDotNetSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "paint.net");
        }
    }

    [Setup("Slack for Windows")]
    public class SlackForWindowsSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "slack");
        }
    }

    [Setup("IrfanView")]
    public class IrfanViewSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "irfanview");
        }
    }

    [Setup("IrfanView all plugins")]
    public class IrfanViewAllPluginsSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "irfanviewplugins");
        }
    }

    [Setup("Gimp")]
    public class GimpSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "gimp");
        }
    }

    [Setup("Beyond Compare 4")]
    public class BeyondCompare4Setup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "beyondcompare");
        }
    }

    [Setup("Dropbox")]
    public class DropboxSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "dropbox");
        }
    }

    [Setup("Adobe Reader DC")]
    public class AdobeReaderDCSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "adobereader");
        }
    }

    [Setup("Foobar 2000")]
    public class Foobar2000Setup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "foobar2000");
        }
    }

    [Setup("Tortoise SVN")]
    public class TortoiseSvnSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "tortoisesvn");
        }
    }

    [Setup("OpenVPN")]
    public class OpenVpnSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "openvpn");
        }
    }

    [Setup("Postman")]
    public class PostmanSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "postman");
        }
    }

    [Setup("VirtualBox")]
    public class VirtualBoxSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "virtualbox");
        }
    }

    [Setup("Telegram")]
    public class TelegramSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "telegram.install");
        }
    }

    [Setup("TeamSpeak 3")]
    public class TeamSpeak3Setup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "teamspeak");
        }
    }

    [Setup("PotPlayer")]
    public class PotPlayerSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "potplayer");
        }
    }

    [Setup("LibreOffice")]
    public class LibreOfficeSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "libreoffice");
        }
    }

    [Setup("NuGet Commandline")]
    public class NuGetCommandlineSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "nuget.commandline");
        }
    }

    [Setup("Dependency Walker", Description = "Inspect Windows modules (.exe, .dll, ...) for their dependencies.")]
    public class DependencyWalkerSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "dependencywalker");
        }
    }

    [Setup("Rapid Environment Editor")]
    public class RapidEnvironmentEditorSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "rapidee");
        }
    }

    [Setup("Screen To Gif")]
    public class ScreenToDifSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "screentogif");
        }
    }

    [Setup("WinDirStat")]
    public class WinDirStatSetup : ISetup
    {
        public void Run(SetupContext context)
        {
            context.ExecuteChocolatey("install", "windirstat");
        }
    }
}

