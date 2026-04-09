using Microsoft.Extensions.Logging;

namespace ProjectXProDash.Services;

public sealed class LoggerService
{
    private readonly ILoggerFactory _loggerFactory;

    public LoggerService(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    public ILogger Create<T>() => _loggerFactory.CreateLogger<T>();
}
