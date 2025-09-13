using Discord;
using Discord.WebSocket;
using System.Text;
using System.Text.Json;

public class TitanCordManager
{
    private DiscordSocketClient _discordSocketClient;
    private HttpClient _httpClient;
    private string _baseUri;

    public TitanCordManager()
    {
        _baseUri = "https://discord.com/api/v10";
        _httpClient = TitanCordUtils.CreateLowLatencyHttpClient();

        _discordSocketClient = new DiscordSocketClient(new DiscordSocketConfig()
        {
            MessageCacheSize = 50,
            GatewayIntents = GatewayIntents.All
        });

        _discordSocketClient.LoginAsync(Discord.TokenType.Bot, TitanCordGlobals.GetBotToken(), true);
        _discordSocketClient.StartAsync();

    }

    public List<ulong> GetAllUserIds(ulong guildId)
    {
        while (true)
        {
            try
            {
                SocketGuild guild = _discordSocketClient.GetGuild(guildId);

                if (guild == null)
                {
                    return new List<ulong>();
                }

                guild.DownloadUsersAsync().GetAwaiter().GetResult();
                List<ulong> userIds = guild.Users.Select(u => u.Id).ToList();
                return userIds;
            }
            catch
            {

            }
        }
    }

    public bool BanUser(string guildId, string userId, string reason = null)
    {
        while (true)
        {
            try
            {
                string url = $"/guilds/{guildId}/bans/{userId}";
                string json = string.IsNullOrEmpty(reason) ? "{}" : $"{{\"reason\":\"{reason}\"}}";
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                while (true)
                {
                    HttpResponseMessage response = _httpClient.PutAsync($"{_baseUri}{url}", content).GetAwaiter().GetResult();

                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }

                    if (TitanCordUtils.IsRateLimit(response))
                    {
                        continue;
                    }

                    if ((int)response.StatusCode == 403)
                    {
                        return false;
                    }

                    if ((int)response.StatusCode == 404)
                    {
                        return false;
                    }

                    return false;
                }
            }
            catch
            {

            }
        }
    }

    public List<DiscordRole> GetAllRoles(string guildId)
    {
        while (true)
        {
            try
            {
                List<DiscordRole> discordRoles = new List<DiscordRole>();
                string url = $"/guilds/{guildId}/roles";

                while (true)
                {
                    HttpResponseMessage response = _httpClient.GetAsync($"{_baseUri}{url}").GetAwaiter().GetResult();
                    string body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (response.IsSuccessStatusCode)
                    {
                        var roles = JsonDocument.Parse(body).RootElement.EnumerateArray();

                        foreach (JsonElement role in roles)
                        {
                            string roleId = role.GetProperty("id").GetString();
                            string roleName = role.GetProperty("name").GetString();
                            int position = role.GetProperty("position").GetInt32();

                            discordRoles.Add(new DiscordRole(roleId, roleName, position));
                        }

                        break;
                    }

                    if (TitanCordUtils.IsRateLimit(response))
                    {
                        TitanCordUtils.SleepForRateLimit(response);
                        continue;
                    }

                    break;
                }

                return discordRoles;
            }
            catch
            {

            }
        }
    }

    public void CreateRole(string guildId, string roleName, int color)
    {
        while (true)
        {
            try
            {
                string url = $"/guilds/{guildId}/roles";

                Dictionary<string, object> jsonObj = new Dictionary<string, object>
                {
                    { "name", roleName },
                    { "color", color }
                };

                string json = JsonSerializer.Serialize(jsonObj);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                while (true)
                {
                    HttpResponseMessage response = _httpClient.PostAsync($"{_baseUri}{url}", content).GetAwaiter().GetResult();
                    string body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (response.IsSuccessStatusCode)
                    {
                        return;
                    }

                    if (TitanCordUtils.IsRateLimit(response))
                    {
                        TitanCordUtils.SleepForRateLimit(response);
                        continue;
                    }

                    if ((int)response.StatusCode == 403)
                    {
                        break;
                    }

                    break;
                }

                break;
            }
            catch
            {

            }
        }
    }

    public void CreateRole(string guildId, string roleName, string colorHex = null, int? position = null, bool hoist = false, bool mentionable = false)
    {
        while (true)
        {
            try
            {
                string url = $"/guilds/{guildId}/roles";

                Dictionary<string, object> jsonObj = new Dictionary<string, object>
                {
                    { "name", roleName },
                    { "hoist", hoist },
                    { "mentionable", mentionable }
                };

                if (!string.IsNullOrEmpty(colorHex))
                {
                    if (colorHex.StartsWith("#"))
                    {
                        colorHex = colorHex.Substring(1);
                    }

                    int colorInt = Convert.ToInt32(colorHex, 16);
                    jsonObj.Add("color", colorInt);
                }

                if (position.HasValue)
                {
                    jsonObj.Add("position", position.Value);
                }

                string json = JsonSerializer.Serialize(jsonObj);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                while (true)
                {
                    HttpResponseMessage response = _httpClient.PostAsync($"{_baseUri}{url}", content).GetAwaiter().GetResult();
                    string body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (response.IsSuccessStatusCode)
                    {
                        break;
                    }

                    if (TitanCordUtils.IsRateLimit(response))
                    {
                        TitanCordUtils.SleepForRateLimit(response);
                        continue;
                    }

                    if ((int)response.StatusCode == 403)
                    {
                        break;
                    }

                    break;
                }

                break;
            }
            catch
            {

            }
        }
    }


    public bool DeleteRole(string guildId, string roleId)
    {
        while (true)
        {
            try
            {
                string url = $"/guilds/{guildId}/roles/{roleId}";

                while (true)
                {
                    HttpResponseMessage response = _httpClient.DeleteAsync($"{_baseUri}{url}").GetAwaiter().GetResult();

                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }

                    if (TitanCordUtils.IsRateLimit(response))
                    {
                        TitanCordUtils.SleepForRateLimit(response);
                        continue;
                    }

                    if ((int)response.StatusCode == 403)
                    {
                        return false;
                    }

                    return false;
                }
            }
            catch
            {

            }
        }   
    }

    public void SendMessage(string channelId, string message)
    {
        while (true)
        {
            try
            {
                message = message.Replace('\r'.ToString(), "\\r").Replace('\n'.ToString(), "\\n");

                string url = $"/channels/{channelId}/messages";
                string json = "{\"content\":\"" + message + "\"}";
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                while (true)
                {
                    HttpResponseMessage response = _httpClient.PostAsync($"{_baseUri}{url}", content).GetAwaiter().GetResult();

                    if (response.IsSuccessStatusCode)
                    {
                        break;
                    }

                    if (TitanCordUtils.IsRateLimit(response))
                    {
                        TitanCordUtils.SleepForRateLimit(response);
                        continue;
                    }

                    break;
                }

                break;
            }
            catch
            {

            }
        }
    }

    public void CreateChannel(string guildId, string channelName, DiscordChannelType channelType)
    {
        while (true)
        {
            try
            {
                string url = $"/guilds/{guildId}/channels";
                string json = $"{{\"name\":\"{channelName}\", \"type\":{((int)channelType).ToString()}}}";
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                while (true)
                {
                    HttpResponseMessage response = _httpClient.PostAsync($"{_baseUri}{url}", content).GetAwaiter().GetResult();
                    string body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (response.IsSuccessStatusCode)
                    {
                        break;
                    }

                    if (TitanCordUtils.IsRateLimit(response))
                    {
                        TitanCordUtils.SleepForRateLimit(response);
                        continue;
                    }

                    break;
                }

                break;
            }
            catch
            {

            }
        }
    }

    public void DeleteChannel(string channelId)
    {
        while (true)
        {
            try
            {
                string url = $"/channels/{channelId}";

                while (true)
                {
                    HttpResponseMessage response = _httpClient.DeleteAsync($"{_baseUri}{url}").GetAwaiter().GetResult();
                    string body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (response.IsSuccessStatusCode)
                    {
                        break;
                    }

                    if (TitanCordUtils.IsRateLimit(response))
                    {
                        TitanCordUtils.SleepForRateLimit(response);
                        continue;
                    }

                    break;
                }

                break;
            }
            catch
            {
                
            }
        }
    }

    public List<DiscordChannel> GetChannels(string guildId)
    {
        while (true)
        {
            try
            {
                List<DiscordChannel> channels = new List<DiscordChannel>();
                string url = $"/guilds/{guildId}/channels";

                HttpResponseMessage response = _httpClient.GetAsync($"{_baseUri}{url}").GetAwaiter().GetResult();
                string json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (!response.IsSuccessStatusCode)
                {
                    return channels;
                }

                JsonDocument doc = JsonDocument.Parse(json);

                foreach (var channel in doc.RootElement.EnumerateArray())
                {
                    string id = channel.GetProperty("id").GetString();
                    string name = channel.GetProperty("name").GetString();
                    int type = channel.GetProperty("type").GetInt32();

                    channels.Add(new DiscordChannel(id, name, (DiscordChannelType)type));
                }

                return channels;
            }
            catch
            {

            }
        }
    }
}