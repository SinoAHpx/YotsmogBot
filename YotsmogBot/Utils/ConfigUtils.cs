using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using AHpx.Extensions.IOExtensions;
using Newtonsoft.Json;
using YotsmogBot.Data.Config;

namespace YotsmogBot.Utils;

public class ConfigUtils
{
    private static ILogger _logger = new Logger();
    
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
            _logger.Log(e);
            return null;
        }
        
    }
    
    public static async Task SaveConfigAsync(Config config)
    {
        try
        {
            var jsonText = JsonConvert.SerializeObject(config);

            _logger.Log($"[green]Config[/] has been save to [green]{ConfigFile.FullName}[/]");
            await ConfigFile.WriteAllTextAsync(jsonText);
        }
        catch (Exception e)
        {
            _logger.Log(e);
        }
    }

    public static async Task<string?> GetApiKeyAsync(string name)
    {
        try
        {
            var config = await GetConfigAsync();
            if (config!.ApiKeys.Any(s => s.Name == name))
                return config.ApiKeys.First(s => s.Name == name).Key;
            
            _logger.Log($"[red]No ApiKey[/] found with name [green]{name}[/]");
        }
        catch (Exception e)
        {
            _logger.Log(e);
        }

        return null;
    }

    public static async Task AddApiKeyAsync(string name, string key)
    {
        try
        {
            var config = await GetConfigAsync();
            if (config!.ApiKeys.Any(s => s.Name == name))
            {
                _logger.Log($"Api key with name [green]{name}[/] already exists");
                return;
            }

            config.ApiKeys.Add(new Config.ApiKey
            {
                Key = key,
                Name = name
            });
            
            await SaveConfigAsync(config);

            _logger.Log($"Api key [green]{name}[/] has been added");
        }
        catch (Exception e)
        {
            _logger.Log(e);
        }
    }

    public static async Task RemoveKeyAsync(string name)
    {
        try
        {
            var config = await GetConfigAsync();
            if (!config!.ApiKeys.Any())
            {
                _logger.Log("No api Key found");
                return;
            }
            if (config!.ApiKeys.All(s => s.Name != name))
            {
                _logger.Log($"Api key with name [green]{name}[/] is not exists");
                return;
            }

            config.ApiKeys.RemoveAll(x => x.Name == name);

            await SaveConfigAsync(config);   
            
            _logger.Log($"Api key [green]{name}[/] has been removed");
        }
        catch (Exception e)
        {
            _logger.Log(e);
        }
    }
}