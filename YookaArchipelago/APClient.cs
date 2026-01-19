using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Il2Cpp;
using MelonLoader;

namespace YookaArchipelago;
using Archipelago.MultiClient.Net;

public class APClient
{
    private static ArchipelagoSession Session;

    public APClient(string host="localhost", int port=38281)
    {
        Session = ArchipelagoSessionFactory.CreateSession(host,port);
        Session.Items.ItemReceived += ReceiveItem;
        
    }

    public void Connect(string player="Player1")
    {
        var loginResult = Session.TryConnectAndLogin("Yooka-Replaylee", player, ItemsHandlingFlags.AllItems, Version.Parse("0.6.5"));
        if (loginResult.Successful)
        {
            var loginSuccess = (LoginSuccessful)loginResult;
            APData.locationsChecked = Session.Locations.AllLocationsChecked.ToList();
        }
        
    }

    public void SendLocation(string location)
    {
        var locationId = Session.Locations.GetLocationIdFromName("Yooka-Replaylee", location);
        if(!APData.locationsChecked.Contains(locationId))
        {
            Melon<YRAPMod>.Logger.Msg($"AP SENDING: {locationId}: {location}");
            APData.locationsChecked.Add(locationId);
            Session.Locations.CompleteLocationChecks(locationId);
        }
    }

    public void ReceiveItem(ReceivedItemsHelper receivedItemsHelper)
    {
        var itemReceivedId = receivedItemsHelper.PeekItem().ItemId;
        var itemReceivedName = receivedItemsHelper.PeekItem().ItemDisplayName;

        Melon<YRAPMod>.Logger.Msg($"AP RECEIVED: {itemReceivedName}:  {itemReceivedId}");
        if(APData.apNameToMoveName.ContainsKey(itemReceivedName))
        {
            APData.AddPlayerMove(itemReceivedName);
        }

        receivedItemsHelper.DequeueItem();
    }

    public static long GetLocationIdFromName(string location)
    {
        return Session.Locations.GetLocationIdFromName("Yooka-Replaylee", location);
    }
    
}