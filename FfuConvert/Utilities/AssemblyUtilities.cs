using System;
using System.IO;
using System.Reflection;

namespace FfuConvert.Utilities
{
    public static class AssemblyUtilities
    {
        public static string GetConfiguration(this Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            object[] atts = assembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);
            if (atts != null && atts.Length > 0)
                return ((AssemblyConfigurationAttribute)atts[0]).Configuration;

            return null;
        }

        public static string GetTitle(this Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            object[] atts = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            if (atts != null && atts.Length > 0)
                return ((AssemblyTitleAttribute)atts[0]).Title;

            return null;
        }

		public static string GetInformationalVersion(this Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            try
            {
                object[] atts = assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
                if ((atts != null) && (atts.Length > 0))
                    return ((AssemblyInformationalVersionAttribute)atts[0]).InformationalVersion;

                return null;
            }
            catch (Exception e)
            {
                return "!" + e.Message + "!";
            }
        }

        public static DateTime? GetLinkerTimestamp(this Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            try
            {
                return GetLinkerTimestamp(assembly.Location);
            }
            catch
            {
                return null;
            }
        }

        public static DateTime? GetLinkerTimestamp(string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            try
            {
                const int peHeaderOffset = 60;
                const int linkerTimestampOffset = 8;
                var bytes = new byte[2048];

                using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    file.Read(bytes, 0, bytes.Length);
                }

                int headerPos = BitConverter.ToInt32(bytes, peHeaderOffset);
                int secondsSince1970 = BitConverter.ToInt32(bytes, headerPos + linkerTimestampOffset);
                var dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                dt = dt.AddSeconds(secondsSince1970);
                dt = dt.ToLocalTime();
                return dt;
            }
            catch
            {
                return null;
            }
        }
    }
}
