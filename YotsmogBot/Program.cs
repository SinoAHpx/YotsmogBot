//First of fucking all, the formal name of this project is 幽小梦山东大奶奶机器人
//The display name of this project is the abbreviation and variety of Yoo tiny mo sand east grand grand mother bot

using System.Reactive.Linq;
using AHpx.Extensions.StringExtensions;
using Mirai.Net.Data.Events.Concretes.Group;
using Mirai.Net.Data.Events.Concretes.Message;
using Mirai.Net.Data.Messages.Concretes;
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

//Main message subscription spreading
var modules = new IntroductionModule().GetModules();
bot.MessageReceived
    .OfType<GroupMessageReceiver>()
    .Subscribe(r =>
    {
        modules.SubscribeModule(r);
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

while (Console.ReadLine() != "exit") { }