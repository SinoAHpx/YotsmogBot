using Mirai.Net.Sessions;

namespace YotsmogBot.Data.Config;

public class Config
{
    public MiraiBot MiraiBot { get; set; }

    public List<ApiKey> ApiKeys { get; set; }
    
    public class ApiKey
    {
        public string Name { get; set; }

        public string Key { get; set; }
    }
}