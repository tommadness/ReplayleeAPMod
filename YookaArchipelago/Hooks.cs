using Il2CppPlaytonic.Game;

namespace YookaArchipelago;
using MelonLoader;
using HarmonyLib;
using Il2Cpp;
using UnityEngine.SceneManagement;
public class Hooks
{
    public delegate void ArchipelagoLocationHandler(string location);
    
    [HarmonyPatch(typeof(PagiePickup))]
    private static class PagiePickupHook
    {
        [HarmonyPatch(nameof(PagiePickup.Collect))]
        [HarmonyPrefix]
        private static bool PagieCollect(PagiePickup __instance)
        {
            string sceneName = SceneManager.GetActiveScene().name;
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
            __instance.MoveGlide.mIsEnabledInGame = false;
        }
    }
    
    [HarmonyPatch(typeof(CoinPickup))]
    public static class CoinPickupHooks
    {
        public static event ArchipelagoLocationHandler CoinCollected;

        [HarmonyPatch(nameof(CoinPickup.GetCollectionStatus))]
        [HarmonyPostfix]
        private static void CoinPickup_GetCollectionStatus(CoinPickup __instance, ref CollectionStatus __result)
        {
            //Melon<YRAPMod>.Logger.Msg(string.Format("{0} COLLECTION STATUS: {1}", __instance.name, __result));
            __result = CollectionStatus.NotSpawned;
        }
        
        [HarmonyPatch(nameof(CoinPickup.Collect))]
        [HarmonyPrefix]
        private static void CoinPickup_Collect(CoinPickup __instance)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            string locationName = string.Format("{0} - {1}", SceneWorld.GetWorld(sceneName), __instance.name);
            //Melon<YRAPMod>.Logger.Msg(locationName);
            CoinCollected(locationName);
        }
    }

}