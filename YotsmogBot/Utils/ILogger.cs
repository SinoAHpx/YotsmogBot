using System.Diagnostics;

namespace YotsmogBot.Utils;

public interface ILogger
{
    public void Log(string message);

    public void Log(Exception exception);
}

public class Logger : ILogger
{
    public void Log(string message)
    {
        AnsiConsole.MarkupLine($@"[red]{DateTime.Now:s}[/] - {message}");
        AnsiConsole.Markup("[red]YotsmogBot[/]> ");
    }

    public void Log(Exception exception)
    {
        AnsiConsole.WriteException(exception, ExceptionFormats.ShortenPaths);
        AnsiConsole.Markup("[red]YotsmogBot[/]> ");
    }
}