﻿using System.Diagnostics;
using System.IO;

namespace CallofDuty4CompileTools.src
{
    class BuildFF
    {
        public static Process LinkerPC = new Process();
        
        /// <summary>
        /// This method starts the build process for the selected map's Fast File (.ff), and Load Fast File (*_load.ff).
        /// </summary>
        public static void Start(string MapName)
        {
            Main.StaticConsoleInstance.WriteLine("\nBuilding Fast Files\n--------------------------------------------------\n");

            LinkerPC.StartInfo.FileName = Utility.GetRootLocation() + @"bin\linker_pc.exe";
            LinkerPC.StartInfo.WorkingDirectory = Utility.GetRootLocation() + @"bin";
            LinkerPC.StartInfo.CreateNoWindow = true;
            LinkerPC.StartInfo.UseShellExecute = false;
            LinkerPC.StartInfo.Arguments = string.Format("{0} {1}", MapName, MapName + "_load");
            LinkerPC.StartInfo.RedirectStandardOutput = true;
            LinkerPC.Start();

            StreamReader Reader = LinkerPC.StandardOutput;
            while (!Reader.EndOfStream)
                Main.StaticConsoleInstance.WriteLine(Reader.ReadLine());

            Reader.Close();
            Reader.Dispose();
            LinkerPC.Close();

            Main.StaticConsoleInstance.WriteLine("\nFinished\n--------------------------------------------------\n");
        }
    }
}