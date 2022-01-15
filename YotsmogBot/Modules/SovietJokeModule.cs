using AHpx.Extensions.StringExtensions;
using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;
using YotsmogBot.Utils;
using YotsmogBot.Utils.Extensions;

namespace YotsmogBot.Modules;

public class SovietJokeModule : IModule
{
    public async void Execute(MessageReceiverBase @base)
    {
        try
        {
            var receiver = @base.Concretize<GroupMessageReceiver>();
            
            if (receiver.MessageChain.ContainsCommand(new[] { "/soviet", "/joke", "来点苏联笑话" }))
            {
                var jokes = ConstantText.ResourceManager
                    .GetString("SovietJoke")
                    .ToJArray()
                    .Select(x => x.ToString())
                    .ToList();
            
                var random = new Random();

                await receiver.SendMessageAsync(jokes[random.Next(jokes.Count)]);
            }
        }
        catch (Exception e)
        {
            new Logger().Log(e);
        }
        
    }

    public bool? IsEnable { get; set; }
}