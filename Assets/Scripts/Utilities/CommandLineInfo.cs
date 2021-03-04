public static class CommandLineInfo
{
    public static int GetTargetScene()
    {

        int sceneIndex;
        if(int.TryParse(GetArg("--activeForgeScene"), out sceneIndex))
        {
            return sceneIndex;
        }
        else
            return -1;
    }

    public static int GetMaxConnections()
    {
        int maxPlayers;
        if(int.TryParse(GetArg("--maxPlayers"), out maxPlayers))
        {
            return maxPlayers;
        }
        else
            return 32;
    }

    private static string GetArg(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return null;
    }
}
