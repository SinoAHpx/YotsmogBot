using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Versioning;
using Flurl.Http;
using Mirai.Net.Data.Messages.Concretes;

namespace YotsmogBot.Modules;

[SupportedOSPlatform("windows")]
[Description("图片放大模块，此模块仅支持windows平台，用法：/picenlarge <图片>")]
public class PicEnlargerModule : IModule
{
    public async void Execute(MessageReceiverBase @base)
    {
        var receiver = @base.Concretize<GroupMessageReceiver>();

        try
        {
            if (receiver.MessageChain.ContainsCommand(new[] { "/picenlarge", "/放大图片" }))
            {
                var images = receiver.MessageChain.OfType<ImageMessage>().ToList();
                var guid = Guid.NewGuid().ToString("N");

                if (!images.Any())
                {
                    await receiver.SendMessageAsync("请在命令后跟随图片");
                    return;
                }

                if (images.Count > 1)
                {
                    await receiver.SendMessageAsync("每次只能放大一张图片");
                    return;
                }

                var imageBytes = await images.First().Url.GetBytesAsync();
                var file = new FileInfo($@"{Directory.GetCurrentDirectory()}\PicEnlarger\temp-{guid}.png");

                if (!file.Directory!.Exists)
                    file.Directory.Create();

                await File.WriteAllBytesAsync($@"{file}", imageBytes);

#pragma warning disable CA1416
                var bitImg = new Bitmap(file.FullName);

                if (bitImg.Width > 320 || bitImg.Height > 320)
                {
                    await receiver.SendMessageAsync(new AtMessage(receiver.Sender.Id).Append(" 此图片已经够大，需要放大的是您的脑子。"));
                    return;
                }
                
                bitImg.Dispose();
#pragma warning restore CA1416
                
                var output = new FileInfo($@"{file.DirectoryName}\{guid}.png");
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo($@"{Directory.GetCurrentDirectory()}\realesrgan\realesrgan-ncnn-vulkan.exe", 
                        $@"-i {file} -o {output}")
                    {
                        CreateNoWindow = true,
                        RedirectStandardOutput = true
                    }
                };
                process.Start();

                while (await process.StandardOutput.ReadLineAsync() != null) { }
                
                await receiver.SendMessageAsync(new ImageMessage
                {
                    Path = output.FullName
                });
                
                output.Directory!.Delete(true);
            }
        }
        catch (Exception e)
        {
            LoggerManager.Log(e);
            await receiver.SendMessageAsync("失败了! 可能是没有部署realesrgan");
        }
    }

    public bool? IsEnable { get; set; }
}