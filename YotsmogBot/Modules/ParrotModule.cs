using System.ComponentModel;
using AHpx.Extensions.StringExtensions;
using Mirai.Net.Data.Messages.Concretes;

namespace YotsmogBot.Modules;

[Description("复读机模块，复读机模块，复读机模块")]
public class ParrotModule : IModule
{
    private static readonly List<IEnumerable<MessageBase>> ParrotList = new();

    public async void Execute(MessageReceiverBase @base)
    {
        var receiver = @base.Concretize<GroupMessageReceiver>();

        try
        {
            var messages = receiver.MessageChain.ToList();
            messages.RemoveAt(0);

            if (!ParrotList.Any())
                ParrotList.Add(messages);

            if (!ParrotList.Any())
                ParrotList.Add(messages);

            var messageJson = messages.Select(MapMessage);
            var parrotList = ParrotList.Select(x => x.Select(MapMessage));
            
            if (parrotList.All(x => x.SequenceEqual(messageJson)))
                ParrotList.Add(messages);
            else
                ParrotList.Clear();

            if (ParrotList.Count > 3)
            {
                await receiver.SendMessageAsync(ParrotList.First().ToArray());
                ParrotList.Clear();
            }
        }
        catch (Exception e)
        {
            LoggerManager.Log(e);
        }
    }

    public string MapMessage(MessageBase @base)
    {
        switch (@base.Type)
        {
            case Messages.Image: return (@base as ImageMessage)!.ImageId;
            case Messages.Plain: return (@base as PlainMessage)!.Text;
            default:
                return @base.ToJsonString();
        }
    }

    public bool? IsEnable { get; set; }
}