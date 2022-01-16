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
        try
        {
            var jsonText = await ConfigFile.ReadAllTextAsync();
            return JsonConvert.DeserializeObject<Config>(jsonText);
        }
        catch (Exception e)
        {
            new Logger().Log(e);
            return null;
        }
        
    }
    
    public static async Task SaveConfigAsync(Config config)
    {
        try
        {
            var jsonText = JsonConvert.SerializeObject(config);

            new Logger().Log($"[green]{jsonText}[/] has been save to [green]{ConfigFile.FullName}[/]");
            await ConfigFile.WriteAllTextAsync(jsonText);
        }
        catch (Exception e)
        {
            new Logger().Log(e);
        }
    }
}