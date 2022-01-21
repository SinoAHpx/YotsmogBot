using System.ComponentModel;
using System.Reflection;
using AHpx.Extensions.StringExtensions;
using YotsmogBot.Utils.Extensions;

namespace YotsmogBot.Modules;

[Description("基础模块，用法：/intro(/help, 帮助)")]
public class IntroductionModule : IModule
{
    public async void Execute(MessageReceiverBase @base)
    {
        try
        {
            var receiver = @base.Concretize<GroupMessageReceiver>();

            if (receiver.MessageChain.ContainsCommand(new[] { "/intro", "/help", "帮助" }))
            {
                var prompt = "这里是幽小梦山东大奶奶机器人!";
                var modules = this.GetModules();

                prompt = modules
                    .Select(x => x.GetType())
                    .Select(x => x.GetCustomAttribute<DescriptionAttribute>()?.Description)
                    .Where(x => x.IsNotNullOrEmpty())
                    .Aggregate(prompt, (c, n) => $"{c}\r\n{n}");
                
                await receiver.SendMessageAsync(prompt);
            }
        }
        catch (Exception e)
        {
            LoggerManager.Log(e);
        }
    }

    public bool? IsEnable { get; set; }
}