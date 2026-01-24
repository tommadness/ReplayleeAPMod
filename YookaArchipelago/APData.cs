using Il2Cpp;
namespace YookaArchipelago;
using MelonLoader;
public static class APData
{
    public delegate void NewMoveHandler(PlayerMoves.Moves move);
    public static event NewMoveHandler NewMoveReceived;
    public static List<long> locationsChecked;
    public static bool DeathlinkReceived = false;
    public static Dictionary<PlayerMoves.Moves, bool> playerMoves = new Dictionary<PlayerMoves.Moves, bool>()
    {
        {PlayerMoves.Moves.BasicAttack,false},
        {PlayerMoves.Moves.Glide,false},
        {PlayerMoves.Moves.Invisibility,false},
        {PlayerMoves.Moves.BasicAttackAir,false},
        {PlayerMoves.Moves.SonarShot,false}, 
        {PlayerMoves.Moves.SonarBoom, false},
        {PlayerMoves.Moves.SonarShield, false},
        {PlayerMoves.Moves.WheelRoll,false},
        {PlayerMoves.Moves.EatMk1,false},
        {PlayerMoves.Moves.EatMk2,false},
        {PlayerMoves.Moves.EatMk3,false},
        {PlayerMoves.Moves.WheelSpinAttack,false},
        {PlayerMoves.Moves.Fly,false},
        {PlayerMoves.Moves.GroundPound,false},
        {PlayerMoves.Moves.HighJump,false},
        {PlayerMoves.Moves.FartBubble,false},
        {PlayerMoves.Moves.TongueGrappleHook,false},
        {PlayerMoves.Moves.WheelDashAttack,false},
        {PlayerMoves.Moves.Jump,false}
};

    public static Dictionary<string, PlayerMoves.Moves> apNameToMoveName = new Dictionary<string, PlayerMoves.Moves>()
    {
        {"Tail Twirl", PlayerMoves.Moves.BasicAttack},
        {"Glide", PlayerMoves.Moves.Glide},
        {"Invisibility", PlayerMoves.Moves.Invisibility},
        {"Aerial Tail Twirl", PlayerMoves.Moves.BasicAttackAir},
        {"Sonar Shot", PlayerMoves.Moves.SonarShot },
        {"Sonar Boom", PlayerMoves.Moves.SonarBoom },
        {"Sonar Shield", PlayerMoves.Moves.SonarShield },
        {"Roll", PlayerMoves.Moves.WheelRoll },
        {"Explosive Shot", PlayerMoves.Moves.EatMk1 },
        {"Ice Shot", PlayerMoves.Moves.EatMk2 },
        {"Flame Shot", PlayerMoves.Moves.EatMk3 },
        {"Wheel Spin Attack", PlayerMoves.Moves.WheelSpinAttack},
        {"Fly", PlayerMoves.Moves.Fly},
        {"Ground Pound", PlayerMoves.Moves.GroundPound },
        {"High Jump", PlayerMoves.Moves.HighJump },
        {"Air Bubble", PlayerMoves.Moves.FartBubble},
        {"Tongue Grapple Hook", PlayerMoves.Moves.TongueGrappleHook},
        {"Wheel Dash Attack", PlayerMoves.Moves.WheelDashAttack},
        {"Jump", PlayerMoves.Moves.Jump},
    };

    public static void AddPlayerMove(string move)
    {
        var moveName = apNameToMoveName[move];
        playerMoves[moveName] = true;
        Melon<YRAPMod>.Logger.Msg($"ADDPLAYERMOVE: {moveName}");
        NewMoveReceived(moveName);
        
    }

    public static int TotalPagies = 300;

}