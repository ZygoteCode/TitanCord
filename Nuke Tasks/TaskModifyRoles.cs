public class TaskModifyRoles
{
    public static void DoTask(string guildId, TitanCordManager _titanCordManager, string roles)
    {
        TitanCordUtils.Log("ROLES", $"Obtaining Role IDs for the guild {guildId}.");
        List<DiscordRole> discordRoles = _titanCordManager.GetAllRoles(guildId);
        string roleIds = "";

        foreach (DiscordRole discordRole in discordRoles)
        {
            if (roleIds == "")
            {
                roleIds = discordRole.Id;
            }
            else
            {
                roleIds += ", " + discordRole.Id;
            }
        }

        TitanCordUtils.Log("ROLES", $"Got the Role IDs for the guild {guildId} ({discordRoles.Count} roles) => [{roleIds}].");
        TitanCordUtils.Log("ROLES", "The objective is now to delete all the roles. Proceeding.");

        int totalRoles = discordRoles.Count, currentDeletedRoles = 0;

        foreach (DiscordRole discordRole in discordRoles)
        {
            Thread thread = new Thread(() =>
            {
                _titanCordManager.DeleteRole(guildId, discordRole.Id);
                TitanCordUtils.Log("ROLES", $"Deleted role {discordRole.Id} successfully.");
                Interlocked.Increment(ref currentDeletedRoles);
            });

            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }

        while (currentDeletedRoles != totalRoles)
        {
            Thread.Sleep(1);
        }

        TitanCordUtils.Log("ROLES", $"Succesfully deleted all the {totalRoles} roles.");

        int rolesNumber = int.Parse(roles);
        TitanCordUtils.Log("ROLES", $"The objective is now to create the request amount of roles ({roles}). Proceeding.");

        for (int i = 0; i < rolesNumber; i++)
        {
            Thread thread = new Thread(() =>
            {
                double hue = i * (360.0 / rolesNumber);
                int colorInt = TitanCordUtils.HslToDiscordInt(hue);
                _titanCordManager.CreateRole(guildId, "☢️ ・ NUKE BY TITANCORD!", colorInt);
                TitanCordUtils.Log("ROLES", "Succesfully created a role!");
            });

            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
    }
}