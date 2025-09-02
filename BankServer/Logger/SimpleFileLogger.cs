using Microsoft.Extensions.Logging;

public class SimpleFileLogger : ILogger//Тоже один из примеров
{
    private readonly string _logFilePath;

    public SimpleFileLogger(string logFilePath)
    {
        _logFilePath = logFilePath;
    }

    public IDisposable? BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)//Здесь уже происходит запись в файл
    {
        //Ну и определение формата записи
        var message = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {formatter(state, exception)}";

        try
        {
            File.AppendAllText(_logFilePath, message + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка записи в лог-файл: {ex.Message}");
        }
    }
}