using Il2CppPlaytonic.Game;
using System.Reflection;
using Il2CppRewiredConsts;

namespace YookaArchipelago;
using MelonLoader;
using HarmonyLib;
using Il2Cpp;
using UnityEngine.SceneManagement;
public class Hooks
{
    public delegate void ArchipelagoLocationHandler(string location);
    public static event ArchipelagoLocationHandler LocationCollected;
    
    [HarmonyPatch(typeof(PagiePickup))]
    private static class PagiePickupHook
    {
        [HarmonyPatch(nameof(PagiePickup.Collect))]
        [HarmonyPrefix]
        private static bool PagieCollect(PagiePickup __instance)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            string locationName = string.Format("{0} - {1}", Data.GetWorld(sceneName), __instance.name);
            LocationCollected(locationName);
            //Melon<YRAPMod>.Logger.Msg(SceneWorld.GetWorld(sceneName) + " - " + __instance.name);
            return true;
        }
        
    }
    [HarmonyPatch(typeof(PlayerMoves))]
    private static class PlayerMovesHook
    {    
        [HarmonyPatch(nameof(PlayerMoves.Start))]
        [HarmonyPostfix]
        private static void CheckPlayerMoves(PlayerMoves __instance)
        {
            foreach (PlayerMoves.Moves move in __instance.mMoveDictionary.Keys)
            {
                __instance.GetMove(move).mIsEnabledInGame = APData.playerMoves[move];
                Melon<YRAPMod>.Logger.Msg($"MOVE: {move}, STATE: {APData.playerMoves[move]}");
            }

        }

        [HarmonyPatch(nameof(PlayerMoves.EnableMoveInGame),new Type[]{typeof(PlayerMoves.Moves), typeof(bool)})]
        [HarmonyPrefix]
        private static bool EnableMoveInGame(PlayerMoves __instance)
        {
            return false;
        }
    }
    
    [HarmonyPatch(typeof(CoinPickup))]
    public static class CoinPickupHooks
    {

        [HarmonyPatch(nameof(CoinPickup.GetCollectionStatus))]
        [HarmonyPostfix]
        private static void CoinPickup_GetCollectionStatus(CoinPickup __instance, ref CollectionStatus __result)
        {
            //Melon<YRAPMod>.Logger.Msg(string.Format("{0} COLLECTION STATUS: {1}", __instance.name, __result));
            string sceneName = __instance.gameObject.scene.name;
            string locationName = string.Format("{0} - {1}", Data.GetWorld(sceneName), __instance.name);
            __result = APData.locationsChecked.Contains(APClient.GetLocationIdFromName(locationName)) ? CollectionStatus.Collected : CollectionStatus.NotSpawned;
        }
        
        [HarmonyPatch(nameof(CoinPickup.Collect))]
        [HarmonyPrefix]
        private static void CoinPickup_Collect(CoinPickup __instance)
        {
            string sceneName = __instance.gameObject.scene.name;
            string locationName = string.Format("{0} - {1}", Data.GetWorld(sceneName), __instance.name);
            //Melon<YRAPMod>.Logger.Msg(locationName);
            LocationCollected(locationName);
        }
    }

    [HarmonyPatch(typeof(PlayerDeathManager))]
    public static class PlayerDeathManagerHooks
    {
        [HarmonyPatch(nameof(PlayerDeathManager.StartPostDeathSequence))]
        [HarmonyPrefix]
        public static bool StartPostDeathSequence(bool allowRespawnToLastSafePosition)
        {
            Melon<YRAPMod>.Logger.Msg("Starting post-death sequence");
            if (APData.DeathlinkReceived)
            {
                Melon<YRAPMod>.Logger.Msg("Not sending deathlink");
                return true;
            }
            Melon<YRAPMod>.Logger.Msg("Sending deathlink");
            APClient.SendDeathlink();
            return true;
        }

        [HarmonyPatch(nameof(PlayerDeathManager.OnFadeInStartEvent))]
        [HarmonyPrefix]
        public static void OnFadeInStartEvent()
        {
            Melon<YRAPMod>.Logger.Msg("Clearing Deathlink Flag");
            APData.DeathlinkReceived = false;
        }
        
    }

}