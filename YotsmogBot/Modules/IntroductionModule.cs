using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;
using YotsmogBot.Utils;
using YotsmogBot.Utils.Extensions;

namespace YotsmogBot.Modules;

public class IntroductionModule : IModule
{
    public async void Execute(MessageReceiverBase @base)
    {
        try
        {
            var receiver = @base.Concretize<GroupMessageReceiver>();

            if (receiver.MessageChain.ContainsCommand(new[] { "/intro", "/help", "帮助" }))
            {
                await receiver.SendMessageAsync("这里是幽小梦山东大奶奶机器人!");
            }
        }
        catch (Exception e)
        {
            new Logger().Log(e);
        }
        
    }

    public bool? IsEnable { get; set; }
}