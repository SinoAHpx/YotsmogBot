using Mirai.Net.Sessions;

namespace YotsmogBot.Data.Config;

public class Config
{
    public MiraiBot MiraiBot { get; set; }

    public List<BlackListEntry> Blacklist { get; set; } = new();
    
    public List<ApiKey> ApiKeys { get; set; } = new();
    
    public class BlackListEntry
    {
        public string Id { get; set; }

        /// <summary>
        /// true: group false: user
        /// </summary>
        public bool Type { get; set; }
    }
    
    public class ApiKey
    {
        public string Name { get; set; }

        public string? Key { get; set; }
    }
}