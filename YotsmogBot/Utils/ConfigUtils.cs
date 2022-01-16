using AHpx.Extensions.IOExtensions;
using Newtonsoft.Json;
using YotsmogBot.Data.Config;

namespace YotsmogBot.Utils;

public class ConfigUtils
{
    private static FileInfo ConfigFile
    {
        get
        {
            var re = new FileInfo($@"{Directory.GetCurrentDirectory()}\Yotsmog\config.json");
            if (!re.Exists)
                re.Directory?.Create();

            return re;
        }
    }

    public static async Task<Config?> GetConfigAsync()
    {
        var jsonText = await ConfigFile.ReadAllTextAsync();
        
        return JsonConvert.DeserializeObject<Config>(jsonText);
    }
    
    public static async Task SaveConfigAsync(Config config)
    {
        var jsonText = JsonConvert.SerializeObject(config);
        await ConfigFile.WriteAllTextAsync(jsonText);
    }
}