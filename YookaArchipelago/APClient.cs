using Il2Cpp;

namespace YookaArchipelago;
using Archipelago.MultiClient.Net;

public class APClient
{
    private ArchipelagoSession Session;

    public APClient(string host, int port)
    {
        Session = ArchipelagoSessionFactory.CreateSession(host,port);
    }
}