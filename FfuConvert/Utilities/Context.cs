using System;
using System.Reflection;

namespace FfuConvert.Utilities
{
    public static class Context
    {
        public static DateTime? AssemblyCompileDate => Assembly.GetExecutingAssembly().GetLinkerTimestamp();
        public static string AssemblyConfiguration => Assembly.GetExecutingAssembly().GetConfiguration();
        public static string Bitness => IntPtr.Size == 4 ? "32-bit" : "64-bit";
        public static Version AssemblyVersion => new Version(Assembly.GetExecutingAssembly().GetInformationalVersion());

        public static string AssemblyDisplayName
        {
            get
            {
                string name = "Version " + AssemblyVersion + " - " + AssemblyConfiguration + " - " + Bitness;
                DateTime? dt = AssemblyCompileDate;
                if (dt.HasValue)
                {
                    name += " - Compiled " + dt.Value;
                }
                return name;
            }
        }
    }
}
