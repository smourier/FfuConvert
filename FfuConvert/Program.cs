using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using FfuConvert.Utilities;

namespace FfuConvert
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Debugger.IsAttached)
            {
                SafeMain(args);
                return;
            }

            try
            {
                SafeMain(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        static void SafeMain(string[] args)
        {
            Console.WriteLine("FfuConvert - Copyright © 2016-" + DateTime.Now.Year + " Simon Mourier. All rights reserved.");
            Console.WriteLine(Context.AssemblyDisplayName);
            Console.WriteLine();
            if (CommandLine.HelpRequested || args.Length < 1)
            {
                Help();
                return;
            }

            string input = CommandLine.GetNullifiedArgument(0);
            string output = CommandLine.GetNullifiedArgument(1);

            using (var file = new FfuFile(input))
            {
                Console.WriteLine("FFU input file: " + input);
                Console.WriteLine(" Catalog Size                     = " + file.SignedCatalog.Length);
                Console.WriteLine(" Chunk Size In Kb                 = " + file.ChunkSizeInKb);
                Console.WriteLine(" Hash Algorithm Type              = " + file.HashAlgorithmType);
                Console.WriteLine(" Hash Table Size                  = " + file.HashTable.Length);
                Console.WriteLine(" Platform Id                      = " + file.PlatformId);
                Console.WriteLine(" FFU Version                      = " + file.Version);
                Console.WriteLine(" Storage Version                  = " + file.StoreVersion);
                Console.WriteLine(" Stores Count                     = " + file.Stores.Count);
                Console.WriteLine(" Block Size In Bytes              =" + file.BlockSizeInBytes);
                Console.WriteLine(" First Store Payload Size         = " + file.FirstStore.PayloadSizeInBytes);
                Console.WriteLine(" First Store Block Count          = " + file.FirstStore.BlockCount);
                Console.WriteLine(" First Store Max Block Index      = " + file.FirstStore.MaxBlockIndex);
                Console.WriteLine(" First Store Target Size In Bytes = " + file.FirstStore.TargetSizeInBytes);
                Console.WriteLine(" Manifest: ");
                Console.WriteLine(IndentLines(2, file.Manifest));

                if (output != null)
                {
                    int percent = 0;
                    file.FirstStore.ProgressChanged += (sender, e) =>
                    {
                        var block = (FfuFileBlock)e.UserState;
                        if (e.ProgressPercentage != percent)
                        {
                            percent = e.ProgressPercentage;
                            if ((percent % 10) == 0)
                            {
                                Console.Write(percent + "%");
                            }
                            else
                            {
                                Console.Write('.');
                            }
                        }
                    };

                    string ext = Path.GetExtension(output).ToLowerInvariant();
                    if (ext == ".img")
                    {
                        file.FirstStore.WriteRaw(output);
                    }
                    else
                    {
                        file.FirstStore.WriteVirtualDisk(output);
                    }
                    Console.WriteLine("100%");
                    Console.WriteLine(output + " file was written successfully.");
                }
            }
        }

        private static string IndentLines(int indent, string text)
        {
            string sindent = new string(' ', indent);
            var sb = new StringBuilder();
            using (var sr = new StringReader(text))
            {
                do
                {
                    string line = sr.ReadLine();
                    if (line == null)
                        break;

                    sb.Append(sindent);
                    sb.AppendLine(line);
                }
                while (true);
            }
            return sb.ToString();
        }

        static void Help()
        {
            Console.WriteLine(Assembly.GetEntryAssembly().GetName().Name.ToUpperInvariant() + " <ffu file path> [output file path]");
            Console.WriteLine();
            Console.WriteLine("Description:");
            Console.WriteLine("    This tool displays information about a FFU (Full Flash Update) file or converts it into a VHDX (Hyper-V virtual disk V2) or IMG (raw) file.");
            Console.WriteLine("    If the target file already exists, it will be overwritten.");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("    " + Assembly.GetEntryAssembly().GetName().Name.ToUpperInvariant() + " flash.ffu");
            Console.WriteLine("    Will display information about flash.ffu file.");
            Console.WriteLine();
            Console.WriteLine("    " + Assembly.GetEntryAssembly().GetName().Name.ToUpperInvariant() + " flash.ffu flash.vhdx");
            Console.WriteLine("    Will convert flash.ffu file into a newly created (or overwritten) flash.vhdx file.");
            Console.WriteLine();
        }
    }
}
