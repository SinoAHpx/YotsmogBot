using AHpx.Extensions.StringExtensions;
using Mirai.Net.Data.Messages;
using Mirai.Net.Utils.Scaffolds;

namespace YotsmogBot.Utils.Extensions;

/// <summary>
/// Pending to be added to main project
/// </summary>
public static class CommandExtensions
{
    public static bool ContainsCommand(this IEnumerable<MessageBase> origin, IEnumerable<string> commands)
    {
        var messageBases = origin.ToList();
        var originInput = messageBases.GetPlainMessage();

        if (originInput.IsNullOrEmpty())
            return false;

        return commands.Any(messageBases.Contains);
    }
}