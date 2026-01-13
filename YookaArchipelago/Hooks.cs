namespace YookaArchipelago;
using MelonLoader;
using HarmonyLib;
using Il2Cpp;
using UnityEngine.SceneManagement;
public class Hooks
{
    [HarmonyPatch(typeof(CoinPickup), "Collect", new Type[] { })]
    private static class QuillPickupHook
    {
        private static bool Prefix(CoinPickup __instance)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            Melon<YRAPMod>.Logger.Msg(YRAPMod.sceneLevel[sceneName] + " - " + __instance.name);
            __instance.SetMaterial(__instance.CollectedMaterial);
            return true;
        }
        
    }
    [HarmonyPatch(typeof(PagiePickup), "Collect", new Type[] { })]
    private static class PagiePickupHook
    {
        private static bool Prefix(PagiePickup __instance)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            Melon<YRAPMod>.Logger.Msg(YRAPMod.sceneLevel[sceneName] + " - " + __instance.name);
            return true;
        }
        
    }
    [HarmonyPatch(typeof(PlayerMoves), "Start", new Type[] { })]
    private static class PlayerMovesHook
    {
        private static void Postfix(PlayerMoves __instance)
        {
            __instance.MoveGlide.mIsEnabledInGame = false;
        }
    }


}