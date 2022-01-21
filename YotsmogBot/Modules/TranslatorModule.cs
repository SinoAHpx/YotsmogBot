using System.ComponentModel;
using AHpx.Extensions.JsonExtensions;
using AHpx.Extensions.StringExtensions;
using Flurl;
using Flurl.Http;
using Mirai.Net.Data.Commands;
using Mirai.Net.Data.Messages.Concretes;

namespace YotsmogBot.Modules;

[Description("翻译模块，基于Azure，用法：/trans -text <内容> -to(可选) <语言>")]
public class TranslatorModule : IModule
{
    public async void Execute(MessageReceiverBase @base)
    {
        var receiver = @base.Concretize<GroupMessageReceiver>();
        
        try
        {
            //todo: add more alias
            if (receiver.MessageChain.ContainsCommand(new[] { "/trans" }))
            {
                var entity = receiver.MessageChain.GetPlainMessage().Trim().ParseCommand<Translation>();

                if (entity.Text.IsNullOrEmpty())
                {
                    await receiver.SendMessageAsync("翻译个啥呀");
                    return;
                }

                if (entity.To.IsNullOrEmpty())
                    entity.To = "zh";

                var result = await TranslateAsync(entity.To, entity.Text);

                await receiver.SendMessageAsync(new AtMessage(receiver.Sender.Id).Append($" {result}"));
            }
        }
        catch (Exception e)
        {
            LoggerManager.Log(e);
            await receiver.SendMessageAsync($"翻译出错了! {e.Message}");
        }
    }

    public bool? IsEnable { get; set; }

    [CommandEntity(Name = "trans")]
    class Translation
    {
        [CommandArgument(Name = "text", IsRequired = true)]
        public string Text { get; set; }
        
        [CommandArgument(Name = "to")]
        public string To { get; set; }
    }
    
    private async Task<string> TranslateAsync(/*string from,*/string to, string origin)
    {
        var raw = await "https://api.cognitive.microsofttranslator.com/translate?api-version=3.0"
            .SetQueryParam("to", to)
            .WithHeader("Ocp-Apim-Subscription-Key", await ConfigUtils.GetApiKeyAsync("azure"))
            .WithHeader("Ocp-Apim-Subscription-Region", "eastasia")
            .PostJsonAsync(new object[]
            {
                new
                {
                    Text = origin
                }
            });

        var response = await raw.GetStringAsync();

        return response
            .ToJArray()
            .First()
            .Fetch("translations")
            .ToJArray()
            .First()
            .Fetch("text");
    }

    // private static string GetLanguage(string origin)
    // {
    //     "https://api.cognitive.microsofttranslator.com/detect?api-version=3.0"
    //         .WithHeader("Ocp-Apim-Subscription-Key", ConfigUtils.GetApiKeyAsync("azure"))
    //         .WithHeader("Ocp-Apim-Subscription-Region", "eastasia")
    //         
    // }
}