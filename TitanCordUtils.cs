using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Drawing;

internal class TitanCordUtils
{
    public static bool IsRateLimit(HttpResponseMessage response)
    {
        return (int)response.StatusCode == 429;
    }

    public static int HslToDiscordInt(double hue, double saturation = 0.9, double lightness = 0.5)
    {
        var color = HslToRgb(hue, saturation, lightness);
        return (color.R << 16) | (color.G << 8) | color.B;
    }

    public static Color HslToRgb(double h, double s, double l)
    {
        h = h / 360.0;
        double r = 0, g = 0, b = 0;

        if (s == 0)
        {
            r = g = b = l;
        }
        else
        {
            double q = l < 0.5 ? l * (1 + s) : l + s - l * s;
            double p = 2 * l - q;
            r = HueToRgb(p, q, h + 1.0 / 3);
            g = HueToRgb(p, q, h);
            b = HueToRgb(p, q, h - 1.0 / 3);
        }

        return Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
    }

    public static double HueToRgb(double p, double q, double t)
    {
        if (t < 0) t += 1;
        if (t > 1) t -= 1;
        if (t < 1.0 / 6) return p + (q - p) * 6 * t;
        if (t < 1.0 / 2) return q;
        if (t < 2.0 / 3) return p + (q - p) * (2.0 / 3 - t) * 6;
        return p;
    }

    public static void SleepForRateLimit(HttpResponseMessage response)
    {
        string body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        using JsonDocument doc = JsonDocument.Parse(body);
        double retryAfter = doc.RootElement.GetProperty("retry_after").GetDouble();
        Thread.Sleep((int)retryAfter);
    }

    public static HttpClient CreateLowLatencyHttpClient()
    {
        SocketsHttpHandler handler = new SocketsHttpHandler
        {
            ConnectCallback = async (ctx, ct) =>
            {
                var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                socket.NoDelay = true;
                await socket.ConnectAsync(ctx.DnsEndPoint, ct);
                return new NetworkStream(socket, ownsSocket: true);
            }
        };

        HttpClient client = new HttpClient(handler, disposeHandler: true);
        client.DefaultRequestHeaders.Add("Authorization", $"Bot {TitanCordGlobals.GetBotToken()}");
        client.DefaultRequestHeaders.Add("User-Agent", "TitanCordBot (https://github.com/yourbot, 1.0)");
        return client;
    }

    public static string CreateMessage(string message)
    {
        int maxLength = 2000;
        StringBuilder sb = new StringBuilder();

        while (sb.Length + message.Length + Environment.NewLine.Length <= maxLength)
        {
            sb.AppendLine(message);
        }

        return sb.ToString();
    }

    public static void Log(string task, string message)
    {
        Console.WriteLine($"[TITANCORD] [{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}] (=> {task} <=) -> {message}");
    }

    public static bool IsGuildIdValid(string guildId)
    {
        if (guildId.Length != 18 && guildId.Length != 19)
        {
            return false;
        }

        ulong guildIdParsed = 0U;
        bool parsedSuccess = ulong.TryParse(guildId, out guildIdParsed);
        return parsedSuccess;
    }

    public static bool IsNumberValid(string channelsNumber)
    {
        uint channelsNumberParsed = 0;
        bool parsedSuccess = uint.TryParse(channelsNumber, out channelsNumberParsed);

        if (!parsedSuccess)
        {
            return false;
        }

        return channelsNumberParsed > 0 && channelsNumberParsed <= 200;
    }

    public static bool IsTextMessageValid(string message)
    {
        return message != null && message.Replace(" ", "").Replace('\t'.ToString(), "").Length > 0;
    }
}