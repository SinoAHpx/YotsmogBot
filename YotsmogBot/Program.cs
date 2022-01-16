//First of fucking all, the formal name of this project is 幽小梦山东大奶奶机器人
//The display name of this project is the abbreviation and variety of Yoo tiny mo sand east grand grand mother bot

using System.Reactive.Linq;
using Mirai.Net.Data.Events.Concretes.Group;
using Mirai.Net.Data.Events.Concretes.Message;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Sessions;
using Mirai.Net.Sessions.Http.Managers;
using YotsmogBot;
using YotsmogBot.Modules;

var bot = new MiraiBot();

AnsiConsole.Write(new Rule("[bold]Welcome to [red]YotsmogBot[/]![/]"));
AnsiConsole.Write(new FigletText("YotsmogBot").Centered().Color(Color.Red));
AnsiConsole.Write(new Rule("[bold][red]Welcome[/] to YotsmogBot![/]"));

//Console commands handler
while (true)
{
    var command = AnsiConsole.Ask<string>("[red]YotsmogBot[/]> ");
    switch (command)
    {
        case "/exit":
            return;
        case "/initialize":
            await InitializeAsync();
            break;
        default:
            AnsiConsole.MarkupLine($"Unknown command: [red]{command}[/]!");
            break;
    }
}

async Task InitializeAsync()
{
    try
    {
        bot.Address = ConfigureBot("Address", "localhost:8080");
        bot.VerifyKey = ConfigureBot("VerifyKey", "1145141919810");
        bot.QQ = ConfigureBot("QQ", "2672886221");

        if (AnsiConsole.Confirm("Run bot?"))
            await LaunchBotAsync();
        else
            AnsiConsole.MarkupLine("You can run bot manually by using [green]/launch[/] command.");
    }
    catch (Exception e)
    {
        new Logger().Log(e);
    }
}

string ConfigureBot(string name, string defaultValue)
{
    return AnsiConsole.Prompt(new TextPrompt<string>($@"Input [green]{name.ToLower()}[/] of mirai-api-http")
        .Validate(s =>
        {
            try
            {
                switch (name)
                {
                    case "Address":
                        bot!.Address = s;
                        break;
                    case "QQ":
                        bot!.QQ = s;
                        break;
                    case "VerifyKey":
                        bot!.VerifyKey = s;
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

async Task LaunchBotAsync()
{
    await AnsiConsole.Status().StartAsync("Initializing...", async context =>
    {
        await bot.LaunchAsync();
    });
    
    //Main message subscription spreading
    var modules = new IntroductionModule().GetModules();
    bot.MessageReceived
        .OfType<GroupMessageReceiver>()
        .Subscribe(r =>
        {
            modules.SubscribeModule(r);
        });
    
    //message output
    bot?.MessageReceived
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
    bot.EventReceived
        .OfType<GroupMessageRecalledEvent>()
        .Subscribe(async e =>
        {
            try
            {
                await e.Group.QuoteGroupMessageAsync(e.MessageId, "不许撤回!");
            }
            catch (Exception exception)
            {
                new Logger().Log(exception);
            }
        });

    //When someone at bot, bot will send a hello to which one
    bot.EventReceived
        .OfType<AtEvent>()
        .Subscribe(async e =>
        {
            await e.Receiver.SendMessageAsync("你好!");
        });

    //Automatically send welcoming message to new members
    bot.EventReceived
        .OfType<MemberJoinedEvent>()
        .Subscribe(e =>
        {
            e.Member.Group.SendGroupMessageAsync("欢迎"
                .Append(new AtMessage(e.Member.Id))
                .Append("进群!"));
        });
}