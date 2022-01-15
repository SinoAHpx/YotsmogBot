using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;

namespace YotsmogBot.Modules;

public class IntroductionModule : IModule
{
    public async void Execute(MessageReceiverBase @base)
    {
        var receiver = @base.Concretize<GroupMessageReceiver>();

        var alias = new[] { "/intro", "/help", "帮助" };
        if (alias.Any(x => receiver.MessageChain.GetPlainMessage().Contains(x)))
        {
            await receiver.SendMessageAsync("这里是幽小梦山东大奶奶机器人!");
        }
    }

    public bool? IsEnable { get; set; }
}