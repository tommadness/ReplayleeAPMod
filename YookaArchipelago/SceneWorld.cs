namespace YookaArchipelago;

public static class SceneWorld
{
    private static Dictionary<string, string> sceneWorld = new Dictionary<string, string>()
    {
        { "Level_01_Jungle", "TT" },
        { "Level_00_Hub_A", "HT" }
    };
    public static string GetWorld(string scene)
    {
        return sceneWorld[scene];
    }
}