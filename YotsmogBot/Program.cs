//First of fucking all, the formal name of this project is 幽小梦山东大奶奶机器人
//The display name of this project is the abbreviation and variety of Yoo tiny mo sand east grand grand mother bot

using Mirai.Net.Sessions;

var bot = new MiraiBot
{
    Address = "localhost:8080",
    VerifyKey = "1145141919810",
    QQ = "2672886221"
};

await bot.LaunchAsync();

while (Console.ReadLine() != "exit") { }