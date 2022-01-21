namespace YotsmogBot.Utils;

public static class LoggerManager
{
    private static ILogger Logger { get; set; } = new Logger();
    
    public static void Log(string message)
    {
        Logger.Log(message);
    }
    
    public static void Log(Exception ex)
    {
        Logger.Log(ex);
    }
}