
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

    [Setup("Lock Hunter")]
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
}

