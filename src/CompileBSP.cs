﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace CallofDuty4CompileTools.src
{
    public static class CompileBSP
    {
        public static Thread Thread { get; set; }
        public static Process CoD4Map { get; } = new Process();
        public static Process CoD4Rad { get; } = new Process();
        public static Process SPTool { get; } = new Process();

        /// <summary>
        /// This method starts the compile process for the selected map's '.d3dbsp' file, for use in game.
        /// </summary>
        /// <param name="BSPPath">The path of the map's '.d3dbsp' file.</param>
        /// <param name="MapSourcePath">The path of the map's '.map' file.</param>
        /// <param name="RootPath">The root directory of the game's installation.</param>
        /// <param name="MapName">The name of the map (without file extension)</param>
        /// <param name="BSPArgs">Any additional arguments to pass the compiler, in terms of the BSP.</param>
        /// <param name="LightArgs">Any additional arguments to pass the compiler, in terms of the Lighting.</param>
        /// <param name="isCompileBSPChecked">Boolean checking if the 'CompileBSPCheckBox' is checked.</param>
        /// <param name="isCompileLightChecked">Boolean checking if the 'CompileLightsCheckBox' is checked</param>
        /// <param name="isCompilePathsChecked">Boolean checking if the 'CompilePathsCheckBox' is checked</param>
        public static void Start(string BSPPath, string MapSourcePath, string RootPath, string MapName, string BSPArgs,
            string LightArgs, bool isCompileBSPChecked, bool isCompileLightChecked, bool isCompilePathsChecked)
        {
            if (isCompileBSPChecked)
            {
                if(Main.StaticMapComboBoxInstance.SelectedItem != null)
                {
                    Main.StaticConsoleInstance.WriteOutputLn("\nCompiling BSP\n--------------------------------------------------", Color.Green);
                    if (!File.Exists(Utility.GetRootLocation() + @"raw\maps\mp\" + MapName + ".map"))
                        File.Copy(MapSourcePath, Utility.GetRootLocation() + @"raw\maps\mp\" + MapName + ".map");

                    CoD4Map.StartInfo.FileName = Utility.GetRootLocation() + @"bin\cod4map.exe";
                    CoD4Map.StartInfo.WorkingDirectory = Utility.GetRootLocation() + @"raw\maps\mp";
                    CoD4Map.StartInfo.CreateNoWindow = true;
                    CoD4Map.StartInfo.UseShellExecute = false;
                    CoD4Map.StartInfo.RedirectStandardOutput = true;
                    CoD4Map.StartInfo.Arguments = string.Format("-platform pc -loadFrom \"{0}\" {1} \"{2}\"", 
                        MapSourcePath, BSPArgs, Path.GetFileNameWithoutExtension(BSPPath));

                    if (File.Exists(CoD4Map.StartInfo.FileName))
                    {
                        CoD4Map.Start();

                        StreamReader Reader = CoD4Map.StandardOutput;
                        while (!Reader.EndOfStream)
                            Main.StaticConsoleInstance.WriteOutputLn(Reader.ReadLine());

                        Reader.Close();
                        Reader.Dispose();
                        CoD4Map.Close();
                    }
                    else
                        Main.StaticConsoleInstance.WriteOutputLn("Error: " + CoD4Map.StartInfo.FileName + " not found!",Color.Red);

                    
                }
                else
                {
                    Main.StaticConsoleInstance.WriteOutputLn("Warning: No Map was Selected. Please Select a Map!", Color.Yellow);
                    return;
                }
            }

            if (isCompileLightChecked)
            {
                Main.StaticConsoleInstance.WriteOutputLn("\nCompiling Lighting\n--------------------------------------------------", Color.Green);
                if (File.Exists(Path.GetFileNameWithoutExtension(MapSourcePath) + ".grid"))
                {
                    File.Copy(Path.GetFileNameWithoutExtension(MapSourcePath) + ".grid",
                        Path.GetFileNameWithoutExtension(BSPPath) + ".grid");
                }

                CoD4Rad.StartInfo.FileName = Utility.GetRootLocation() + @"bin\cod4rad.exe";
                CoD4Rad.StartInfo.WorkingDirectory = Utility.GetRootLocation() + @"raw\maps\mp";
                CoD4Rad.StartInfo.CreateNoWindow = true;
                CoD4Rad.StartInfo.UseShellExecute = false;
                CoD4Rad.StartInfo.RedirectStandardOutput = true;
                CoD4Rad.StartInfo.Arguments = string.Format("-platform pc {0} \"{1}\"",
                    LightArgs, Path.GetFileNameWithoutExtension(BSPPath));

                if (File.Exists(CoD4Rad.StartInfo.FileName))
                {
                    CoD4Rad.Start();

                    StreamReader Reader = CoD4Rad.StandardOutput;
                    while (!Reader.EndOfStream)
                        Main.StaticConsoleInstance.WriteOutputLn(Reader.ReadLine());

                    Reader.Close();
                    Reader.Dispose();
                    CoD4Rad.Close();
                }
                else
                    Main.StaticConsoleInstance.WriteOutputLn("Error: " + CoD4Rad.StartInfo.FileName + " not found!", Color.Red);

                
            }

            string[] DelSearchPatterns = new string[] { ".map", ".d3dprt", ".d3dpoly", ".vclog", ".grid"};
            List<string> FilesToDelete = Directory.GetFiles(Utility.GetRootLocation() + @"raw\maps\mp\", "*.*")
                    .Where(item => DelSearchPatterns.Any(format => Regex.IsMatch(item, format + "$"))).ToList();

            foreach (string file in FilesToDelete ?? Enumerable.Empty<string>())
            {
                if (File.Exists(file))
                    File.Delete(file);
            }

            if (File.Exists(Path.GetFileNameWithoutExtension(BSPPath) + ".lin"))
            {
                File.Move(Path.GetFileNameWithoutExtension(BSPPath) + ".lin",
                    Path.GetFileNameWithoutExtension(MapSourcePath) + ".lin");
            }

            if(isCompilePathsChecked)
            {
                Main.StaticConsoleInstance.WriteOutputLn("\nCompiling Paths\n--------------------------------------------------", Color.Green);
                
                SPTool.StartInfo.FileName = Utility.GetRootLocation() + @"sp_tool.exe";
                SPTool.StartInfo.WorkingDirectory = Utility.GetRootLocation();
                SPTool.StartInfo.CreateNoWindow = true;
                SPTool.StartInfo.UseShellExecute = false;
                SPTool.StartInfo.RedirectStandardOutput = true;
                SPTool.StartInfo.Arguments = string.Format("+set r_fullscreen 0 +set logfile 2 +set monkeytoy 0 " +
                    "+set com_introplayed 1 +set usefastfile 0 +set g_connectpaths 2 +devmap {0}", MapName);

                if (File.Exists(SPTool.StartInfo.FileName))
                {
                    SPTool.Start();

                    StreamReader Reader = SPTool.StandardOutput;
                    while (!Reader.EndOfStream)
                        Main.StaticConsoleInstance.WriteOutputLn(Reader.ReadLine());

                    Reader.Close();
                    Reader.Dispose();
                    SPTool.Close();
                }
                else
                    Main.StaticConsoleInstance.WriteOutputLn("Error: " + SPTool.StartInfo.FileName + " not found!", Color.Red); 
            }

            Main.StaticConsoleInstance.WriteOutputLn("\nFinished\n--------------------------------------------------\n", Color.Green);
            Thread.Abort();
        }
    }
}