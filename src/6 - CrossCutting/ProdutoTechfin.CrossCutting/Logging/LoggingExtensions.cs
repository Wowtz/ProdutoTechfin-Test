using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace ProdutoTechfin.CrossCutting.Logging;

public static class LoggingExtensions
{
    /// <summary>
    /// Loga chamada de serviço a partir de um controller — útil para rastrear
    /// o fluxo controller → use case no DataDog.
    /// </summary>
    public static void LogServiceCall(
        this ILogger logger,
        string controllerName,
        string serviceName,
        string methodName,
        params object[] args)
    {
        if (!logger.IsEnabled(LogLevel.Debug))
            return;

        logger.LogDebug(
            "Controller {Controller} chamou {Service}.{Method} com {@Args}",
            controllerName, serviceName, methodName, args);
    }

    /// <summary>
    /// Loga início de operação com timestamp — útil para medir tempo
    /// de operações específicas fora do pipeline HTTP.
    /// </summary>
    public static IDisposable? BeginOperationScope(
        this ILogger logger,
        string operationName,
        [CallerMemberName] string callerName = "")
    {
        return logger.BeginScope(new Dictionary<string, object?>
        {
            ["Operation"] = operationName,
            ["CallerMember"] = callerName,
            ["StartedAt"] = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Loga resultado de operação com nível dinâmico baseado no sucesso.
    /// </summary>
    public static void LogOperationResult(
        this ILogger logger,
        string operationName,
        bool success,
        long elapsedMs,
        string? detail = null)
    {
        if (success)
        {
            logger.LogInformation(
                "Operation {Operation} completed successfully in {Elapsed}ms. {Detail}",
                operationName, elapsedMs, detail ?? string.Empty);
        }
        else
        {
            logger.LogWarning(
                "Operation {Operation} failed after {Elapsed}ms. {Detail}",
                operationName, elapsedMs, detail ?? string.Empty);
        }
    }
}