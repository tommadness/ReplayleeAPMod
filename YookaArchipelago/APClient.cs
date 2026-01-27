using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.MessageLog.Parts;
using Il2Cpp;
using MelonLoader;

namespace YookaArchipelago;

using Archipelago.MultiClient.Net;

public class APClient
{
    private static ArchipelagoSession Session;
    public static DeathLinkService DeathlinkService;

    public APClient(string host = "localhost", int port = 38281)
    {
        Session = ArchipelagoSessionFactory.CreateSession(host, port);
        Session.Items.ItemReceived += ReceiveItem;
        Session.MessageLog.OnMessageReceived += ReceiveMessage;
        Melon<YRAPMod>.Logger.Msg($"Created Session");

    }

    public void Connect(string player = "Player1", bool enableDeathlink = true)
    {
        Melon<YRAPMod>.Logger.Msg($"Connecting to Session as {player}");
        var loginResult = Session.TryConnectAndLogin("Yooka-Replaylee", player, ItemsHandlingFlags.AllItems, Version.Parse("0.6.5"));
        if (loginResult.Successful)
        {
            var loginSuccess = (LoginSuccessful)loginResult;
            APData.locationsChecked = Session.Locations.AllLocationsChecked.ToList();
            DeathlinkService = Session.CreateDeathLinkService();
            if (enableDeathlink)
            {
                DeathlinkService.EnableDeathLink();
                Melon<YRAPMod>.Logger.Msg("Deathlink enabled");
            }
            else
            {
                Melon<YRAPMod>.Logger.Msg("Deathlink disabled");
            }
        }
        else
        {
            var loginFailure = (LoginFailure)loginResult;
            Melon<YRAPMod>.Logger.Msg($"Failed to connect: {loginFailure.Errors[0]}");
        }

    }

    public static void SendLocation(string location)
    {
        //Melon<YRAPMod>.Logger.Msg($"Sending location: {location}");
        var locationId = Session.Locations.GetLocationIdFromName("Yooka-Replaylee", location);
        if (!APData.locationsChecked.Contains(locationId))
        {
            //Melon<YRAPMod>.Logger.Msg($"AP SENDING: {locationId}: {location}");
            APData.locationsChecked.Add(locationId);
            Session.Locations.CompleteLocationChecks(locationId);
        }
    }

    public void ReceiveItem(ReceivedItemsHelper receivedItemsHelper)
    {
        var itemReceivedId = receivedItemsHelper.PeekItem().ItemId;
        var itemReceivedName = receivedItemsHelper.PeekItem().ItemDisplayName;

        //Melon<YRAPMod>.Logger.Msg($"AP RECEIVED: {itemReceivedName}:  {itemReceivedId}");
        if (Data.apNameToMoveName.ContainsKey(itemReceivedName))
        {
            APData.AddPlayerMove(itemReceivedName);
        }

        receivedItemsHelper.DequeueItem();
    }

    public static void ReceiveMessage(LogMessage message)
    {
        switch (message)
        {
            case PlayerSpecificLogMessage playerMessage:
                if (!playerMessage.IsActivePlayer)
                {
                    return;
                }
                Melon<YRAPMod>.Logger.Msg(playerMessage.ToString());
                break;
            default:
                Melon<YRAPMod>.Logger.Msg(message.ToString());
                break;
        }
        
    }

    public static long GetLocationIdFromName(string location)
    {
        return Session.Locations.GetLocationIdFromName("Yooka-Replaylee", location);
    }

    public static void SendDeathlink()
    {
        var deathlink = new DeathLink("Player1");
        DeathlinkService.SendDeathLink(deathlink);
    }

    public void ToggleDeathlink(bool enable)
    {
        if (enable)
        {
            DeathlinkService.EnableDeathLink();
            Melon<YRAPMod>.Logger.Msg("Deathlink enabled");
        }
        else
        {
            DeathlinkService.DisableDeathLink();
            Melon<YRAPMod>.Logger.Msg("Deathlink disabled");
        }
    }

}