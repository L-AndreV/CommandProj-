using Microsoft.Extensions.Logging;

public static class LoggingExtensions
{
    public static ILoggingBuilder AddSimpleFile(this ILoggingBuilder builder, string filePath)
    {
        builder.AddProvider(new SimpleFileLoggerProvider(filePath));
        return builder;
    }
}