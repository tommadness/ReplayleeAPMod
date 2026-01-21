using System.Collections;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using MelonLoader;
using Il2Cpp;
using Il2CppPlaytonic.Game;
using UnityEngine;
using Il2CppParadoxNotion;
using UnityEngine.SceneManagement;


namespace YookaArchipelago
{
    public static class BuildInfo
    {
        public const string Name = "Yooka-Replaylee Archipelago"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "Yooka-Replaylee Archipelago connector mod"; // Description for the Mod.  (Set as null if none)
        public const string Author = "tommadness"; // Author of the Mod.  (MUST BE SET)
        public const string Company = null!; // Company that made the Mod.  (Set as null if none)
        public const string Version = "0.0.1"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null!; // Download Link for the Mod.  (Set as null if none)
    }

    public class YRAPMod : MelonMod
    {
        public static Hooks hooks = null!;
        private APClient _client = null!;
        private GameObject player = null!;
        private PlayerDeathManager deathManager = null!;
        
        public override void OnEarlyInitializeMelon()
        {
            Application.runInBackground = true;
        }
        
        public override void OnLateInitializeMelon()
        {
            _client = new APClient();
            _client.Connect();
            hooks = new Hooks();
            
            Hooks.LocationCollected += _client.SendLocation;
            APData.NewMoveReceived += ActivatePlayerMove;
            APClient.DeathlinkService.OnDeathLinkReceived += ReceiveDeathlink;

        }

        private void ReceiveDeathlink(DeathLink deathLink)
        {
            LoggerInstance.Msg("Received death link");
            LoggerInstance.Msg("Finding deathManager");
            LoggerInstance.Msg($"Found: {deathManager.name}");
            LoggerInstance.Msg($"DeathManagerPlayer: {deathManager.mPlayer.name}");
            LoggerInstance.Msg("Setting debounce flag true");
            APData.DeathlinkReceived = true;
            LoggerInstance.Msg("Calling Post Death Sequence");
            MelonCoroutines.Start(DoDeathLink());
        }

        public void ActivatePlayerMove(PlayerMoves.Moves move)
        {
            findPlayer();
            LoggerInstance.Msg($"ACTIVAING: {move}");
            player.GetComponent<PlayerMoves>().GetMove(move).mIsEnabledInGame = true;
        }

        private void findPlayer()
        {
            if (player == null)
            {
                player = GameObject.Find("PlayerKamBatV5");
                LoggerInstance.Msg(player.name);
            }
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if(sceneName == "Level_00_Hub_A_Environment")
            {
                MelonCoroutines.Start(UnslipEarlySlopes());
            }

            if (sceneName == "Level_00_Hub_A_CaveJ")
            {
                MelonCoroutines.Start(SkipTutorialCave());
            }

            if (sceneName == "Level_Common")
            {
                MelonCoroutines.Start(FindDeathManager());
            }
        }

        private IEnumerator UnslipEarlySlopes()
        {           
            yield return new WaitForSeconds(1f);
            GameObject.Find("hub_lair_floor_slippy_01_a").GetComponent<ObjectSurface>().IsSurfaceSlippy = false;
        }

        private IEnumerator SkipTutorialCave()
        {
            yield return new WaitForSeconds(5f);
            LoggerInstance.Msg("Setting up Tutorial Cave skip");
            var caveToHTDoor =
                GameObject.Find(
                    "DOORS/CaveExitToHivoryEntranceDoor");
            LoggerInstance.Msg(caveToHTDoor.name);
            var shipwreckToCaveDoor = GameObject.Find("DOORS/ShipwreckCreekToCaveEntranceDoor");
            LoggerInstance.Msg(shipwreckToCaveDoor.name);
            caveToHTDoor.GetComponent<Transform>().position = shipwreckToCaveDoor.GetComponent<Transform>().position;
            caveToHTDoor.GetComponent<Transform>().rotation = shipwreckToCaveDoor.GetComponent<Transform>().rotation;
            shipwreckToCaveDoor.SetActive(false);


        }

        private IEnumerator FindDeathManager()
        {
            yield return new WaitForSeconds(0.1f);
            LoggerInstance.Msg("Finding deathManager");
            deathManager = GameObject.Find("PlayerDeathManager").GetComponent<PlayerDeathManager>();
            LoggerInstance.Msg($"Found: {deathManager.name}");
        }

        private IEnumerator DoDeathLink()
        {
                yield return new WaitForSeconds(0.1f);
                deathManager.StartPostDeathSequence(false);
        }
        
        
        
    }
}