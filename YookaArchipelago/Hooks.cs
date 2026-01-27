using Il2CppPlaytonic.Game;
using System.Reflection;
using Il2CppPlaytonic.Core;
using Il2CppRewiredConsts;
using System.Collections;
using System.Net.Mime;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

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
        [HarmonyPatch(nameof(PagiePickup.GetCollectionStatus))]
        [HarmonyPostfix]
        private static void CoinPickup_GetCollectionStatus(PagiePickup __instance, ref CollectionStatus __result)
        {
            string sceneName = __instance.gameObject.scene.name;
            string locationName = string.Format("{0} - {1}", Data.GetWorld(sceneName), __instance.name);
            __result = APData.locationsChecked.Contains(APClient.GetLocationIdFromName(locationName)) ? CollectionStatus.Collected : CollectionStatus.NotSpawned;
        }
        
        [HarmonyPatch(nameof(PagiePickup.Collect))]
        [HarmonyPrefix]
        private static bool PagieCollect(PagiePickup __instance)
        {
            string sceneName = __instance.gameObject.scene.name;
            Melon<YRAPMod>.Logger.Msg($"Pagie collected in scene:{sceneName}, object name: {__instance.name}");
            PagieChallengeData challenge = __instance.PagieChallengeData;
            string locationName = $"{Data.GetWorld(sceneName)} - {challenge.PagieName.GetLocalizedString()}";
            Melon<YRAPMod>.Logger.Msg($"{locationName}");
            if (!Data.apLocations.ContainsKey(sceneName))
            {
                Data.apLocations.Add(sceneName, new List<string>());
            }

            if (!Data.apLocations[sceneName].Contains(locationName))
            {
                Data.apLocations[sceneName].Add(locationName);
            }
            File.WriteAllText("aplocations.json", JsonConvert.SerializeObject(Data.apLocations,Formatting.Indented));

            LocationCollected(locationName);
            __instance.PlayCollectEffects();
            __instance.gameObject.SetActive(false);
            return false;
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
            string sceneName = __instance.gameObject.scene.name;
            string locationName = string.Format("{0} - {1}", Data.GetWorld(sceneName), __instance.name);
            __result = APData.locationsChecked.Contains(APClient.GetLocationIdFromName(locationName)) ? CollectionStatus.Collected : CollectionStatus.NotSpawned;
        }
        
        [HarmonyPatch(nameof(CoinPickup.Collect))]
        [HarmonyPrefix]
        private static void CoinPickup_Collect(CoinPickup __instance)
        {
            string sceneName = __instance.gameObject.scene.name;
            string locationName = $"{Data.GetWorld(sceneName)} - {__instance.name}";
            if (!Data.apLocations.ContainsKey(sceneName))
            {
                Data.apLocations.Add(sceneName, new List<string>());
            }
            if (!Data.apLocations[sceneName].Contains(locationName))
            {
                Data.apLocations[sceneName].Add(locationName);
            }
            File.WriteAllText("aplocations.json", JsonConvert.SerializeObject(Data.apLocations,Formatting.Indented));
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

    [HarmonyPatch(typeof(TotalPagiesHudDataSource))]
    public static class PagieTotalHook
    {
        [HarmonyPatch(nameof(TotalPagiesHudDataSource.Value), MethodType.Getter)]
        [HarmonyPostfix]
        private static void HudDataSourceValueGetter(TotalPagiesHudDataSource __instance, ref int __result)
        {
            //Melon<YRAPMod>.Logger.Msg(APData.TotalPagies);
            __result = APData.TotalPagies;
        }
    }

    [HarmonyPatch(typeof(FrontendControllerBase))]
    public static class GameFrontendControllerHooks
    {
        [HarmonyPatch(nameof(FrontendControllerBase.Start))]
        [HarmonyPrefix]
        private static void Start(GameFrontendController __instance)
        {
            var apButton = __instance.transform.Find("MainMenuScreen.UI/Content/Buttons/Wishlist");
            var text = apButton.GetComponent<MainMenuItemController>().ItemTitle;
            text.Text = "Archipelago";
            
            apButton.gameObject.SetActive(true);
            //Melon<YRAPMod>.Logger.Msg(apButton.name);
            //Melon<YRAPMod>.Logger.Msg(text);
        }
    }

    [HarmonyPatch(typeof(LocalisationHelper))]
    public static class LocalisationHelperHooks
    {
        [HarmonyPatch(nameof(LocalisationHelper.OnTextReloadedEvent))]
        [HarmonyPostfix]
        private static void SetText(LocalisationHelper __instance)
        {
            string parentName = __instance.transform.parent.parent.name;
            if (parentName == "Wishlist")
            {
                Melon<YRAPMod>.Logger.Msg("Changing Wishlist text");
                __instance.SetText("Archipelago");
            }
            
        }
    }

    [HarmonyPatch(typeof(UnityEngine.EventSystems.EventTrigger))]
    public static class EventTriggerHooks
    {
        [HarmonyPatch(nameof(UnityEngine.EventSystems.EventTrigger.OnSubmit))]
        [HarmonyPrefix]
        private static bool OnSubmit(UnityEngine.EventSystems.EventTrigger __instance)
        {
            if (__instance.gameObject.name == "Wishlist")
            {
                YRAPMod.gui.ToggleAPUI();
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(GameStatManager))]
    public static class GameStatManagerHooks
    {
        [HarmonyPatch(nameof(GameStatManager.Start))]
        [HarmonyPostfix]
        private static void GetCurrentValue(GameStatManager __instance)
        {
            Melon<YRAPMod>.Logger.Msg($"Started Game Stat Manager");
        }
    }

}