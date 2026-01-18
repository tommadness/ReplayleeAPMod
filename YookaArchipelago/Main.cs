using System.Collections;
using MelonLoader;
using Il2Cpp;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;


namespace YookaArchipelago
{
    public static class BuildInfo
    {
        public const string Name = "Yooka-Replaylee Archipelago"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "Yooka-Replaylee Archipelago connector mod"; // Description for the Mod.  (Set as null if none)
        public const string Author = "tommadness"; // Author of the Mod.  (MUST BE SET)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "0.0.1"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class YRAPMod : MelonMod
    {
        public static Hooks Hooks = null!;
        private APClient _client;
        
        public override void OnEarlyInitializeMelon()
        {
            Application.runInBackground = true;
        }
        
        public override void OnLateInitializeMelon()
        {
            _client = new APClient("localhost", 38281);
            Hooks = new Hooks();

        }
        
        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if(sceneName == "Level_00_Hub_A_Environment")
            {
                MelonCoroutines.Start(UnslipEarlySlopes());
            }
        }

        private IEnumerator UnslipEarlySlopes()
        {           
            yield return new WaitForSeconds(1f);
            GameObject.Find("hub_lair_floor_slippy_01_a").GetComponent<ObjectSurface>().IsSurfaceSlippy = false;
        }
        
    }
}