namespace steam_dl_daemon;

public class Game
{
    public string Username { get; set; }
    public uint AppId { get; set; }
    public string Platform { get; set; }
    public string Architecture { get; set; }
    public string Language { get; set; }
    public string Output { get; set; }
    public int[] Ignore { get; set; }
}