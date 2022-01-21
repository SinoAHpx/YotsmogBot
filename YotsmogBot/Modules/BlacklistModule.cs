using System.ComponentModel;
using AHpx.Extensions.StringExtensions;
using Mirai.Net.Data.Commands;
using Mirai.Net.Data.Messages.Concretes;

namespace YotsmogBot.Modules;

[Description("黑名单模块，把某人或者或群加入/移除黑名单，用法: /block(unblock) -user(-group) value")]
public class BlacklistModule : IModule
{
    public async void Execute(MessageReceiverBase @base)
    {
        var receiver = @base.Concretize<GroupMessageReceiver>();

        try
        {
            //todo: bot administrator needed
            if (receiver.Sender.Id == "2933170747" && receiver.MessageChain.GetPlainMessage().IsNotNullOrEmpty())
            {
                var plain = receiver.MessageChain.GetPlainMessage().Trim();

                if (receiver.MessageChain.OfType<AtMessage>().Any())
                    plain += receiver.MessageChain.OfType<AtMessage>().First().Target;


                if (plain.StartsWith("/block"))
                {
                    var block = plain.ParseCommand<BlackList>();
                    if (block.Group.IsNotNullOrEmpty())
                    {
                        await ConfigUtils.AddBlackListAsync(block.Group, true);

                        if (receiver.Id == block.Group)
                            await receiver.SendMessageAsync("本群已被屏蔽!");
                        else
                            await receiver.SendMessageAsync($"群{block.Group}已被屏蔽!");
                    }
                    if (block.User.IsNotNullOrEmpty())
                    {
                        await ConfigUtils.AddBlackListAsync(block.User, false);

                        if (receiver.Sender.Id == block.User)
                            await receiver.SendMessageAsync("你已被屏蔽!");
                        else
                            await receiver.SendMessageAsync($"{block.User}已被屏蔽!");
                    }
                }
                
                if (plain.StartsWith("/unblock"))
                {
                    var unblock = plain.ParseCommand<UnblockList>();
                    if (unblock.Group.IsNotNullOrEmpty())
                    {
                        await ConfigUtils.RemoveBlackListAsync(unblock.Group);

                        if (receiver.Id == unblock.Group)
                            await receiver.SendMessageAsync("本群已被解除屏蔽!");
                        else
                            await receiver.SendMessageAsync($"群{unblock.Group}已被解除屏蔽!");
                    }
                    if (unblock.User.IsNotNullOrEmpty())
                    {
                        await ConfigUtils.RemoveBlackListAsync(unblock.User);

                        if (receiver.Sender.Id == unblock.User)
                            await receiver.SendMessageAsync("你已被解除屏蔽!");
                        else
                            await receiver.SendMessageAsync($"{unblock.User}已被解除屏蔽!");
                    }
                }
            }
        }
        catch (Exception e)
        {
            LoggerManager.Log(e);
        }
    }

    public bool? IsEnable { get; set; }

    [CommandEntity(Name = "block")]
    class BlackList
    {
        [CommandArgument(Name = "group")]
        public string Group { get; set; }

        [CommandArgument(Name = "user")]
        public string User { get; set; }
    }
    
    [CommandEntity(Name = "unblock")]
    class UnblockList
    {
        [CommandArgument(Name = "group")]
        public string Group { get; set; }

        [CommandArgument(Name = "user")]
        public string User { get; set; }
    }
}