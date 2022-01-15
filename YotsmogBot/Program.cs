//First of fucking all, the formal name of this project is 幽小梦山东大奶奶机器人
//The display name of this project is the abbreviation and variety of Yoo tiny mo sand east grand grand mother bot

using System.Reactive.Linq;
using Mirai.Net.Data.Events.Concretes.Group;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using YotsmogBot;
using YotsmogBot.Modules;
using YotsmogBot.Utils;

var bot = new MiraiBot
{
    Address = "localhost:8080",
    VerifyKey = "1145141919810",
    QQ = "2672886221"
};

await bot.LaunchAsync();

var modules = new IntroductionModule().GetModules();
bot.MessageReceived
    .OfType<GroupMessageReceiver>()
    .Subscribe(r =>
    {
        modules.SubscribeModule(r);
    });

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

while (Console.ReadLine() != "exit") { }