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
        Debug.WriteLine($"{DateTime.Now:T} - {message}");
    }
    
    public void Log(string message, Exception exception)
    {
        Debug.WriteLine($"{DateTime.Now:T} - exception \"{exception.Message}\" occured: {message}");
    }

    public void Log(Exception exception)
    {
        Debug.WriteLine($"{DateTime.Now:T} - exception \"{exception.Message}\" occured");
    }
}