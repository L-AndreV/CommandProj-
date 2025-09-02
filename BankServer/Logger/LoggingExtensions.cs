using Microsoft.Extensions.Logging;
//Это я взял из одного из примеров
//Поэтому не могу сказать, как это работает
//Скорее всего логирование сделано по шаблону

public static class LoggingExtensions
{
    public static ILoggingBuilder AddSimpleFile(this ILoggingBuilder builder, string filePath)
    {
        builder.AddProvider(new SimpleFileLoggerProvider(filePath));
        return builder;
    }
}