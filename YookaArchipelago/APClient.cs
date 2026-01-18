using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Il2Cpp;
using MelonLoader;

namespace YookaArchipelago;
using Archipelago.MultiClient.Net;

public class APClient
{
    private ArchipelagoSession Session;
    private APData? _apData;

    public APClient(string host="localhost", int port=38281)
    {
        Session = ArchipelagoSessionFactory.CreateSession(host,port);
        Session.Items.ItemReceived += ReceiveItem;
        
    }

    public APData? Connect(string player="Player1")
    {
        var loginResult = Session.TryConnectAndLogin("Yooka-Replaylee", player, ItemsHandlingFlags.AllItems, Version.Parse("0.6.5"));
        if (loginResult.Successful)
        {
            _apData = new APData();
            var loginSuccess = (LoginSuccessful)loginResult;
            _apData.locationsChecked = Session.Locations.AllLocationsChecked.ToList();
            return _apData;
        }

        return null;
    }

    public void SendLocation(string location)
    {
        var locationId = Session.Locations.GetLocationIdFromName("Yooka-Replaylee", location);
        if(!_apData.locationsChecked.Contains(locationId))
        {
            Melon<YRAPMod>.Logger.Msg($"AP SENDING: {locationId}: {location}");
            _apData.locationsChecked.Add(locationId);
            Session.Locations.CompleteLocationChecks(locationId);
        }
    }

    public void ReceiveItem(ReceivedItemsHelper receivedItemsHelper)
    {
        var itemReceivedId = receivedItemsHelper.PeekItem().ItemId;
        var itemReceivedName = receivedItemsHelper.PeekItem().ItemDisplayName;

        Melon<YRAPMod>.Logger.Msg($"AP RECEIVED: {itemReceivedName}:  {itemReceivedId}");

        receivedItemsHelper.DequeueItem();
    }
}