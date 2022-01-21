using System.ComponentModel;
using AHpx.Extensions.StringExtensions;
using YotsmogBot.Utils.Extensions;

namespace YotsmogBot.Modules;

[Description("苏联笑话模块，用法：/soviet(/joke, 来点苏联笑话)")]
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
            LoggerManager.Log(e);
        }
    }

    public bool? IsEnable { get; set; }
}