//First of fucking all, the formal name of this project is 幽小梦山东大奶奶机器人
//The display name of this project is the abbreviation and variety of Yoo tiny mo sand east grand grand mother bot

//TODO: this project will be a template

using System.ComponentModel;
using System.Reactive.Linq;
using System.Reflection;
using AHpx.Extensions.StringExtensions;
using Mirai.Net.Data.Events.Concretes.Group;
using Mirai.Net.Data.Events.Concretes.Message;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Sessions;
using Mirai.Net.Sessions.Http.Managers;
using YotsmogBot.Data.Config;
using YotsmogBot.Modules;

namespace YotsmogBot;

class Program
{
    private static MiraiBot _bot = new MiraiBot();

    static async Task Main(string[] args)
    {
        #region Welcome

        AnsiConsole.Write(new Rule("[bold]Welcome to [red]YotsmogBot[/]![/]"));
        AnsiConsole.Write(new FigletText("YotsmogBot").Centered().Color(Color.Red));
        AnsiConsole.Write(new Rule("[bold][red]Welcome[/] to YotsmogBot![/]"));

        #endregion
        
        //Console commands handler
        while (true)
        {
            var command = AnsiConsole.Ask<string>("[red]YotsmogBot[/]> ");

            try
            {
                await ExecuteCommandHandlerAsync(command);
            }
            catch (Exception e)
            {
                LoggerManager.Log(e);
            }
        }
    }
    
    
    #region Commands

    [Description("Exit the bot")]
    public static void Exit()
    {
        Environment.Exit(0);
    }
    
    [Description("Initialize the bot")]
    public static async Task InitializeAsync()
    {
        _bot.Address = ConfigureBot("Address", "localhost:8080");
        _bot.VerifyKey = ConfigureBot("VerifyKey", "1145141919810");
        _bot.QQ = ConfigureBot("QQ", "2672886221");

        if (AnsiConsole.Confirm("Run bot?"))
            await LaunchBotAsync();
        else
            AnsiConsole.MarkupLine("You can run bot manually by using [green]/launch[/] command.");

        await ConfigUtils.SaveConfigAsync(new Config()
        {
            MiraiBot = _bot
        });
    }

    [Description("Launch the bot")]
    public static async Task LaunchAsync()
    {
        var config = await ConfigUtils.GetConfigAsync();

        if (config is null)
            AnsiConsole.MarkupLine("Please [green]/initialize first![/]");
        else
            _bot = config.MiraiBot;

        await LaunchBotAsync();
    }

    [Description("Add a key")]
    public static async Task AddKeyAsync()
    {
        var name = AnsiConsole.Prompt(new TextPrompt<string>("Name of the [green]key[/]: "));
        var key = AnsiConsole.Prompt(new TextPrompt<string>("[green]Key[/]: ").Secret());

        await ConfigUtils.AddApiKeyAsync(name, key);
    }

    [Description("Show config")]
    public static async Task ShowConfigAsync()
    {
        var config = await ConfigUtils.GetConfigAsync();

        AnsiConsole.MarkupLine($"[green]{config.ToJsonString().EscapeMarkup()}[/]");
    }

    [Description("List keys")]
    public static async Task ListKeysAsync()
    {
        var config = await ConfigUtils.GetConfigAsync();

        if (config!.ApiKeys.Any())
            AnsiConsole.MarkupLine(
                $"{config.ApiKeys.Select(x => $"[green]{x.Name}[/]-{x.Key}").Aggregate((a, b) => $"{a}{Environment.NewLine}{b}")}");
        else
            AnsiConsole.MarkupLine("No keys found.");
    }

    [Description("Show help")]
    public static void Help()
    {
        var promptText = GetCommandHandlers()
            .Select(x =>
                $"[green]/{x.Name.ToLower().Empty("async")}[/] - {x.GetCustomAttribute<DescriptionAttribute>()?.Description}")
            .Aggregate((a, b) => $"{a}{Environment.NewLine}{b}");

        AnsiConsole.MarkupLine(promptText);
    }

    [Description("Remove an unwanted key")]
    public static async Task RemoveKeyAsync()
    {
        await ListKeysAsync();

        var config = await ConfigUtils.GetConfigAsync();
        if (config!.ApiKeys.Any())
        {
            var name = AnsiConsole.Ask<string>("Name of the [green]key[/] for removing: ");

            await ConfigUtils.RemoveKeyAsync(name);
        }
    }

    [Description("Block an user")]
    public static async Task BlockUserAsync()
    {
        var id = AnsiConsole.Ask<long>("Which [green]user[/] do you want to block? ");

        await ConfigUtils.AddBlackListAsync(id.ToString(), false);
    }
    
    [Description("Block a group")]
    public static async Task BlockGroupAsync()
    {
        var id = AnsiConsole.Ask<long>("Which [green]group[/] do you want to block? ");

        await ConfigUtils.AddBlackListAsync(id.ToString(), true);
    }

    [Description("Show blocked entries")]
    public static async Task ShowBlackListAsync()
    {
        var config = await ConfigUtils.GetConfigAsync();

        if (config!.Blacklist.Any())
            config!.Blacklist
                .Select(x => $"[green]{x.Id}[/] - {(x.Type ? "Group" : "User")}")
                .ToList()
                .ForEach(AnsiConsole.MarkupLine);
        else
            AnsiConsole.MarkupLine("No blocked entries found.");
    }
    
    [Description("Unblock from blacklist")]
    public static async Task UnblockAsync()
    {
        await ShowBlackListAsync();
        var config = await ConfigUtils.GetConfigAsync();

        if (config!.Blacklist.Any())
        {
            var id = AnsiConsole.Ask<long>("Which [green]user[/] or [green]group[/] do you want to unblock? ");
            await ConfigUtils.RemoveBlackListAsync(id.ToString());
        }
    }
    
    #endregion

    #region Helpers

    private static string ConfigureBot(string name, string defaultValue)
    {
        return AnsiConsole.Prompt(new TextPrompt<string>($@"Input [green]{name.ToLower()}[/] of mirai-api-http")
            .Validate(s =>
            {
                try
                {
                    switch (name)
                    {
                        case "Address":
                            _bot!.Address = s;
                            break;
                        case "QQ":
                            _bot!.QQ = s;
                            break;
                        case "VerifyKey":
                            _bot!.VerifyKey = s;
                            break;
                        default:
                            return ValidationResult.Error($"[red]Invalid {name}![/]");
                    }

                    return ValidationResult.Success();
                }
                catch
                {
                    return ValidationResult.Error($"[red]Invalid {name}![/]");
                }
            })
            .DefaultValue(defaultValue));
    }

    private static async Task LaunchBotAsync()
    {
        await AnsiConsole.Status().StartAsync("Initializing...", async context => { await _bot.LaunchAsync(); });

        //Main message subscription spreading
        var modules = new IntroductionModule().GetModules();
        _bot.MessageReceived
            .OfType<GroupMessageReceiver>()
            .Subscribe(async r =>
            {
                try
                {                    
                    var config = await ConfigUtils.GetConfigAsync();

                    if (config.Blacklist.Where(x => x.Type).Select(x => x.Id).Contains(r.Id) ||
                        config.Blacklist.Where(x => !x.Type).Select(x => x.Id).Contains(r.Sender.Id))
                        return;
                    
                    modules.SubscribeModule(r);
                }
                catch (Exception e)
                {
                    LoggerManager.Log(e);
                }
            });

        //message output
        _bot.MessageReceived
            .OfType<GroupMessageReceiver>()
            .Subscribe(r =>
            {
                var promptMessage = r.MessageChain.GetPlainMessage();

                promptMessage = r.MessageChain
                    .ToList()
                    .Where(x => x.Type != Messages.Plain && x.Type != Messages.Source)
                    .Select(x => $"[[{x.Type}]]")
                    .Aggregate(promptMessage, (s, s1) => s + s1);

                AnsiConsole.MarkupLine(
                    $"{DateTime.Now:s} [[{r.Name}({r.Id})]] {r.Sender.Name}({r.Sender.Id}) -> {promptMessage}");
                AnsiConsole.Markup("[red]YotsmogBot[/]> ");
            });

        //Anti recall
        _bot.EventReceived
            .OfType<GroupMessageRecalledEvent>()
            .Subscribe(async e =>
            {
                try
                {
                    await e.Group.QuoteGroupMessageAsync(e.MessageId, "不许撤回!");
                }
                catch (Exception exception)
                {
                    LoggerManager.Log(exception);
                }
            });

        //When someone at bot, bot will send a hello to which one
        _bot.EventReceived
            .OfType<AtEvent>()
            .Subscribe(async e => { await e.Receiver.SendMessageAsync("你好!"); });

        //Automatically send welcoming message to new members
        _bot.EventReceived
            .OfType<MemberJoinedEvent>()
            .Subscribe(e =>
            {
                e.Member.Group.SendGroupMessageAsync("欢迎"
                    .Append(new AtMessage(e.Member.Id))
                    .Append("进群!"));
            });
    }

    private static IEnumerable<MethodInfo> GetCommandHandlers()
    {
        return typeof(Program)
            .GetMethods()
            .Where(x => x.IsPublic && x.GetCustomAttribute<DescriptionAttribute>() != null);
    }

    private static async Task ExecuteCommandHandlerAsync(string command)
    {
        foreach (var methodInfo in GetCommandHandlers())
        {
            if (methodInfo.Name.ToLower().Empty("async") == command.Trim().TrimStart('/'))
            {
                if (methodInfo.Name.Contains("Async"))
                    await (Task)methodInfo.Invoke(null, null)!;
                else
                    methodInfo.Invoke(null, null);
                
                return;
            }
        }
        
        AnsiConsole.MarkupLine($"Unknown command: [red]{command}[/]! Use [green]/help[/] to see the list of commands.");
    }

    private static async Task WriteLine(string content)
    {
        await Task.Run(() => AnsiConsole.MarkupLine(content));
    }

    #endregion
}
