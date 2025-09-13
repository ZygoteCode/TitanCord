public class DiscordChannel
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public DiscordChannelType Type { get; private set; }

    public DiscordChannel(string id, string name, DiscordChannelType type)
    {
        Id = id;
        Name = name;
        Type = type;
    }
}