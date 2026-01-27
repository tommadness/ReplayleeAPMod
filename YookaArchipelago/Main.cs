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
        private APClient _client = null!;
        private GameObject player = null!;
        private PlayerDeathManager deathManager = null!;
        public static ArchipelagoGUI gui = null!;

        public override void OnEarlyInitializeMelon()
        {
            Application.runInBackground = true;
        }

        public override void OnLateInitializeMelon()
        {
            gui = new ArchipelagoGUI();
            gui.SetMoveToggledCallback(ActivatePlayerMove);
            APData.NewMoveReceived += ActivatePlayerMove;
            APClient.DeathlinkService.OnDeathLinkReceived += ReceiveDeathlink;
        }

        private void ReceiveDeathlink(DeathLink deathLink)
        {
            APData.DeathlinkReceived = true;
            MelonCoroutines.Start(DoDeathLink());
        }

        public void ActivatePlayerMove(PlayerMoves.Moves move)
        {
            findPlayer();
            player.GetComponent<PlayerMoves>().GetMove(move).mIsEnabledInGame = APData.playerMoves[move];
        }

        private void findPlayer()
        {
            if (player == null)
            {
                player = GameObject.Find("PlayerKamBatV5");
            }
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (sceneName == "Level_00_Hub_A_Environment")
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

        public override void OnLateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                gui.ToggleAPUI();
            }

#if DEBUG
            if (Input.GetKeyDown(KeyCode.F3))
            {
                gui.ToggleDebugMenu();
            }
#endif
        }
    }
}