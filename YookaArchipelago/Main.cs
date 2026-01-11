using System.Runtime.CompilerServices;
using MelonLoader;
using Il2Cpp;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using Archipelago.MultiClient.Net;
using Il2CppNodeCanvas.Tasks.Actions;
using UnityEngine.Windows;


namespace YookaArchipelago
{
    public static class BuildInfo
    {
        public const string Name = "Yooka-Replaylee Archipelago"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "Yooka-Replaylee Archipelago connector mod"; // Description for the Mod.  (Set as null if none)
        public const string Author = "tommadness"; // Author of the Mod.  (MUST BE SET)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "0.0.1a"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class YRAPMod : MelonMod
    {
        private static Dictionary<string, string> _sceneLevel = new Dictionary<string, string>();
        
        [HarmonyPatch(typeof(CoinPickup), "Collect", new Type[] { })]
        private static class QuillPickup
        {
            private static void Prefix(CoinPickup __instance)
            {
                string sceneName = SceneManager.GetActiveScene().name;
                Melon<YRAPMod>.Logger.Msg(_sceneLevel[sceneName] + " - " + __instance.name);
            }
    
            private static void Postfix()
            {
                
            }
        }
        [HarmonyPatch(typeof(PagiePickup), "Collect", new Type[] { })]
        private static class PagePickup
        {
            private static bool Prefix(PagiePickup __instance)
            {
                string sceneName = SceneManager.GetActiveScene().name;
                Melon<YRAPMod>.Logger.Msg(_sceneLevel[sceneName] + " - " + __instance.name);
                __instance.GetCollectionStatus();
                return false;
            }
    
            private static void Postfix()
            {
                
            }
        }

        public override void OnLateInitializeMelon()
        {
            Application.runInBackground = true;
            _sceneLevel.Add("Level_01_Jungle", "TT");
            LoggerInstance.Msg("Yooka-Replaylee Archipelago Loaded");
            
        }
    }
}