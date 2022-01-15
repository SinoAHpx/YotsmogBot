//First of fucking all, the formal name of this project is 幽小梦山东大奶奶机器人
//The display name of this project is the abbreviation and variety of Yoo tiny mo sand east grand grand mother bot

using System.Reactive.Linq;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions;
using Mirai.Net.Utils.Scaffolds;
using YotsmogBot.Modules;

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

while (Console.ReadLine() != "exit") { }