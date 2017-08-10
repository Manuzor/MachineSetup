using System;
using System.Reflection;

public static partial class Global
{
    public static readonly Assembly ThisAssembly = Assembly.GetAssembly(typeof(Global));
    public static string GlobalName => ThisAssembly.GetName().Name;
    public static Version GlobalVersion => ThisAssembly.GetName().Version;
}
