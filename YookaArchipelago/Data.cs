namespace YookaArchipelago;

public static class Data
{
    private static Dictionary<string, string> sceneWorld = new Dictionary<string, string>()
    {   
        { "Level_00", "HT" },
        { "Level_01", "TT" },
        {"Level_02", "GlGl"},
        {"Level_03", "MM"},
        {"Level_04", "CC"},
        {"Level_05", "GaGa"},
    };
    public static string GetWorld(string scene)
    {
        scene = scene.Substring(0, 8);
        return sceneWorld[scene];
    }
}