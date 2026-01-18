using Archipelago.MultiClient.Net.Enums;
using Il2Cpp;
using MelonLoader;

namespace YookaArchipelago;
using Archipelago.MultiClient.Net;

public class APClient
{
    private ArchipelagoSession Session;

    public APClient(string host, int port)
    {
        Session = ArchipelagoSessionFactory.CreateSession(host,port);
        Session.Items.ItemReceived += (receivedItemsHelper) =>
        {
            var itemReceivedName = receivedItemsHelper.PeekItem().ItemDisplayName;

            Melon<YRAPMod>.Logger.Msg($"AP RECEIVED: {itemReceivedName}");

            receivedItemsHelper.DequeueItem();
        };
        Session.TryConnectAndLogin("Yooka-Replaylee", "Player1", ItemsHandlingFlags.AllItems, Version.Parse("0.6.5"),
            null, null, null, true);
    }

    public void SendLocation(string location)
    {
        var locationId = Session.Locations.GetLocationIdFromName("Yooka-Replaylee", location);
        Melon<YRAPMod>.Logger.Msg($"AP SENDING: {locationId}: {location}");
        Session.Locations.CompleteLocationChecks(locationId);
    }
}