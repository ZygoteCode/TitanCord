public class DiscordRole
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public int Position { get; private set; }

    public DiscordRole(string id, string name, int position)
    {
        Id = id;
        Name = name;
        Position = position;
    }
}