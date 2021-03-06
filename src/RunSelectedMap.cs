﻿using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace CallofDuty4CompileTools
{
    public static class RunSelectedMap
    {
        public static Process GameClient { get; } = new Process();

        /// <summary>
        /// Starts the selected map using the game client process.
        /// </summary>
        /// <param name="MapName">The name of the map (without file extension)</param>
        /// <param name="isMultiplayerMap">Boolean value determining if the map is meant for Single Player, or Multiplayer.</param>
        /// <param name="isDeveloperChecked">Boolean value determining whether the developer checkbox is checked</param>
        /// /// <param name="isDeveloperScriptChecked">Boolean value determining whether the developer script checkbox is checked</param>
        /// /// <param name="isCheatsChecked">Boolean value determining whether the cheats checkbox is checked</param>
        /// <param name="Args">
        ///     The arguments passed to the game client, if the 'CustomCommandLineCheckBox' is checked,
        ///     which are defined by whatever string is inside the 'MapCustomCommandLineTextBox' Text Box.
        /// </param>
        public static void Start(string MapName, bool isMultiplayerMap, bool isDeveloperChecked,
            bool isDeveloperScriptChecked, bool isCheatsChecked, string Args)
        {
            string Executable = isMultiplayerMap ? "iw3mp.exe" : "iw3sp.exe";
            string Developer = isDeveloperChecked ? "+developer 1" : null;
            string DeveloperScript = isDeveloperScriptChecked ? "+developer_script 1" : null; 
            string Cheats = isCheatsChecked ? "+set sv_cheats 1" : null;

            GameClient.Initialize(Executable, Utility.GetRootLocation(),
                string.Format("{0} {1} {2} {3} +devmap {4}", Developer,
                DeveloperScript, Cheats, Args, MapName.Substring(0, MapName.Length - 4)));
            GameClient.StartInfo.CreateNoWindow = false;

            if (File.Exists(GameClient.StartInfo.FileName))
                GameClient.Start();
            else
                Main.StaticConsoleInstance.WriteOutputLn("Error: " + GameClient.StartInfo.FileName + " not found!", Color.Red);

            Main.StaticConsoleInstance.WriteOutputLn("Running " + MapName + "\n--------------------------------------------------", Color.CornflowerBlue);
            Main.StaticConsoleInstance.WriteOutputLn(Executable + " " + GameClient.StartInfo.Arguments);
        }
    }
}
