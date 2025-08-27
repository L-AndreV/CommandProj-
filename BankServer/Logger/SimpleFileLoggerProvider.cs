using Microsoft.Extensions.Logging;

public class SimpleFileLoggerProvider : ILoggerProvider
{
    private readonly string _logFilePath;

    public SimpleFileLoggerProvider(string logFilePath)
    {
        _logFilePath = logFilePath;
    }

    public ILogger CreateLogger(string categoryName) => new SimpleFileLogger(_logFilePath);

    public void Dispose() { }
}