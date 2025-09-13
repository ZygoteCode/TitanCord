public class TitanCordGlobals
{
    private static string _botToken;

    public static void SetBotToken(string botToken)
    {
        _botToken = botToken;
    }

    public static string GetBotToken()
    {
        return _botToken;
    }
}