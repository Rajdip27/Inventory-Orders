using Microsoft.Extensions.Logging;

namespace InventoryOrders.Application.Logging;

public interface IAppLogger<T>
{
    void LogInfo(string message);
    void LogWarning(string message);
    void LogError(string message, Exception ex);
}
public class AppLogger<T>(ILogger<T> logger) : IAppLogger<T>
{
    public void LogInfo(string message)=>
        logger.LogInformation(message);

    public void LogWarning(string message)=>
        logger.LogWarning(message);

    public void LogError(string message, Exception ex)=>
        logger.LogError(ex, message);
}
