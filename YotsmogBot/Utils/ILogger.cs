using System.Diagnostics;

namespace YotsmogBot.Utils;

public interface ILogger
{
    public void Log(string message);

    public void Log(string message, Exception exception);
    public void Log(Exception exception);
}

public class Logger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine($"{DateTime.Now:T} - {message}");
    }
    
    public void Log(string message, Exception exception)
    {
        Console.WriteLine($"{DateTime.Now:T} - Exception \"{exception.Message}\" occured from {exception.StackTrace}: {message} ");
    }

    public void Log(Exception exception)
    {
        Console.WriteLine($"{DateTime.Now:T} - Exception \"{exception.Message}\" occured from {exception.StackTrace}");
    }
}