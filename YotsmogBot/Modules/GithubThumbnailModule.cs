using System.Xml;
using AHpx.Extensions.StringExtensions;
using Flurl.Http;
using HtmlAgilityPack;
using Mirai.Net.Data.Messages.Concretes;

namespace YotsmogBot.Modules;

public class GithubThumbnailModule : IModule
{
    public async void Execute(MessageReceiverBase @base)
    {
        try
        {
            var receiver = @base.Concretize<GroupMessageReceiver>();
            var url = string.Empty;

            if (receiver.MessageChain.OfType<XmlMessage>().Any())
            {
                var xmlStr = receiver.MessageChain.OfType<XmlMessage>().First().Xml;
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlStr);

                var node = xmlDoc.SelectSingleNode("msg[url~=github]");
                url = node?.Attributes?["url"]?.Value;
            }
            if (receiver.MessageChain.GetPlainMessage()?.Contains("github.com") is true)
            {
                url = receiver.MessageChain.GetPlainMessage().Trim();
            }

            if (url == string.Empty) 
                return;
            
            var response = await url.GetStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(response);

            var imageUrl = doc.DocumentNode
                .SelectSingleNode("//meta[@property='og:image']")
                .Attributes["content"].Value;

            await receiver.SendMessageAsync(new ImageMessage
            {
                Url = imageUrl
            });
        }
        catch (Exception e)
        {
            new Logger().Log(e);
        }
    }

    public bool? IsEnable { get; set; }
}