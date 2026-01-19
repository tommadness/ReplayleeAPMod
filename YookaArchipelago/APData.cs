using Il2Cpp;

namespace YookaArchipelago;

public static class APData
{
    public delegate void NewMoveHandler(string move);
    public static event NewMoveHandler NewMoveReceived;
    public static List<long> locationsChecked;
    public static Dictionary<string, bool> playerMoves = new Dictionary<string, bool>()
    {
        {"MoveBasicAttack",false},
        {"MoveGlide",false}
    };

    private static Dictionary<string, string> apNameToMoveName = new Dictionary<string, string>()
    {
        { "Tail Twirl", "MoveBasicAttack" },
        { "Glide", "MoveGlide" },
    };

    public static void AddPlayerMove(string move)
    {
        var moveName = apNameToMoveName[move];
        playerMoves[moveName] = true;
        NewMoveReceived(moveName);
        
    }

}