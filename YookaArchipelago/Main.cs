using MelonLoader;

using UnityEngine;



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
        public static Dictionary<string, string> sceneLevel = new Dictionary<string, string>();
        private GameObject player;
        private Hooks _hooks = new Hooks();
        private APClient _client;
        
        public override void OnLateInitializeMelon()
        {
            sceneLevel.Add("Level_01_Jungle", "TT");
            LoggerInstance.Msg("Yooka-Replaylee Archipelago Loaded");
            _client = new APClient("localhost", 38281);
        }

        public override void OnEarlyInitializeMelon()
        {
            Application.runInBackground = true;
        }
        
    }
}