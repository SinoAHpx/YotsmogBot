using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
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
            LoggerManager.Log(e);
            return null;
        }
        
    }
    
    public static async Task SaveConfigAsync(Config config)
    {
        try
        {
            var jsonText = JsonConvert.SerializeObject(config);

            LoggerManager.Log($"[green]Config[/] has been save to [green]{ConfigFile.FullName}[/]");
            await ConfigFile.WriteAllTextAsync(jsonText);
        }
        catch (Exception e)
        {
            LoggerManager.Log(e);
        }
    }

    public static async Task<string?> GetApiKeyAsync(string name)
    {
        try
        {
            var config = await GetConfigAsync();
            if (config!.ApiKeys.Any(s => s.Name == name))
                return config.ApiKeys.First(s => s.Name == name).Key;
            
            LoggerManager.Log($"[red]No ApiKey[/] found with name [green]{name}[/]");
        }
        catch (Exception e)
        {
            LoggerManager.Log(e);
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
                LoggerManager.Log($"Api key with name [green]{name}[/] already exists");
                return;
            }

            config.ApiKeys.Add(new Config.ApiKey
            {
                Key = key,
                Name = name
            });
            
            await SaveConfigAsync(config);

            LoggerManager.Log($"Api key [green]{name}[/] has been added");
        }
        catch (Exception e)
        {
            LoggerManager.Log(e);
        }
    }

    public static async Task RemoveKeyAsync(string name)
    {
        try
        {
            var config = await GetConfigAsync();
            if (!config!.ApiKeys.Any())
            {
                LoggerManager.Log("No api Key found");
                return;
            }
            if (config!.ApiKeys.All(s => s.Name != name))
            {
                LoggerManager.Log($"Api key with name [green]{name}[/] is not exists");
                return;
            }

            config.ApiKeys.RemoveAll(x => x.Name == name);

            await SaveConfigAsync(config);   
            
            LoggerManager.Log($"Api key [green]{name}[/] has been removed");
        }
        catch (Exception e)
        {
            LoggerManager.Log(e);
        }
    }

    public static async Task AddBlackListAsync(string id, bool type)
    {
        try
        {
            var config = await GetConfigAsync();
            config!.Blacklist.Add(new Config.BlackListEntry
            {
                Id = id,
                Type = type
            });
            
            await SaveConfigAsync(config);
            LoggerManager.Log($"Blacklist {(type ? "group" : "user")} [green]{id}[/] has been added");
        }
        catch (Exception e)
        {
            LoggerManager.Log(e);
        }
    }
    
    public static async Task RemoveBlackListAsync(string id)
    {
        try
        {
            var config = await GetConfigAsync();
            
            if (config!.Blacklist.Any(x => x.Id == id))
            {
                config!.Blacklist.RemoveAll(x => x.Id == id);

                await SaveConfigAsync(config);
                LoggerManager.Log($"Blacklist [green]{id}[/] has been unblocked");
            }
            else
                LoggerManager.Log("No blacklist found");
        }
        catch (Exception e)
        {
            LoggerManager.Log(e);
        }
    }
}