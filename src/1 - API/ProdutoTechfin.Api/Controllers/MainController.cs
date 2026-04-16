using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProdutoTechfin.CrossCutting.Logging;

namespace ProdutoTechfin.Api.Controllers;

[ExcludeFromCodeCoverage]
[ApiController]
public abstract class MainController<T> : ControllerBase, IAsyncActionFilter where T : ControllerBase
{
    protected readonly ILogger<MainController<T>> _logger;

    protected MainController(ILogger<MainController<T>> logger)
    {
        _logger = logger;
    }

    [NonAction]
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            var resultContext = await next();
            sw.Stop();

            if (resultContext.Exception != null && !resultContext.ExceptionHandled)
            {
                var controller = typeof(T).Name;
                var action = GetActionName(context);
                var traceId = GetTraceId();

                using var scope = CreateScope(controller, action, traceId);
                _logger.LogError(resultContext.Exception,
                    "Exceção em {Controller}.{Action} após {Elapsed}ms",
                    controller, action, sw.ElapsedMilliseconds);

                return; // deixa o middleware tratar
            }

            if (resultContext != null)
            {
                var status = resultContext.HttpContext.Response.StatusCode;
                var elapsed = sw.ElapsedMilliseconds;
                var shouldLog = ShouldLog(status, elapsed, out var logLevel);

                if (shouldLog)
                {
                    var controller = typeof(T).Name;
                    var action = GetActionName(context);
                    var traceId = GetTraceId();

                    using var scope = CreateScope(controller, action, traceId);
                    LogRequest(controller, action, status, elapsed, logLevel, context);
                }
            }
        }
        catch (Exception ex)
        {
            sw.Stop();

            var controller = typeof(T).Name;
            var action = GetActionName(context);
            var traceId = GetTraceId();

            using var scope = CreateScope(controller, action, traceId);
            _logger.LogError(ex,
                "Exceção em {Controller}.{Action} após {Elapsed}ms",
                controller, action, sw.ElapsedMilliseconds);

            throw; 
        }
    }

    protected void LogServiceCall(string serviceName, string methodName, params object[] args)
    {
        _logger.LogServiceCall(typeof(T).Name, serviceName, methodName, args);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string GetActionName(ActionExecutingContext context)
        => context.ActionDescriptor.DisplayName ?? "UnknownAction";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string GetTraceId()
        => Activity.Current?.Id ?? string.Empty;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private IDisposable? CreateScope(string controller, string action, string traceId)
    {
        return _logger.BeginScope(new Dictionary<string, object?>
        {
            ["Controller"] = controller,
            ["Action"] = action,
            ["TraceId"] = traceId
        });
    }

    private bool ShouldLog(int status, long elapsed, out LogLevel level)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            level = LogLevel.Debug;
            return true;
        }

        if (status >= 500)
        {
            level = LogLevel.Error;
            return true;
        }

        if (status >= 400 && status != 404)
        {
            level = LogLevel.Warning;
            return true;
        }

        if (elapsed > 15000)
        {
            level = LogLevel.Warning;
            return true;
        }

        level = LogLevel.None;
        return false;
    }

    private void LogRequest(string controller, string action, int status, long elapsed, LogLevel level, ActionExecutingContext context)
    {
        switch (level)
        {
            case LogLevel.Error:
                _logger.LogError(
                    "{Controller}.{Action} retornou {Status} em {Elapsed}ms",
                    controller, action, status, elapsed);
                break;

            case LogLevel.Warning when elapsed > 15000:
                _logger.LogWarning(
                    "{Controller}.{Action} foi lento: {Status} em {Elapsed}ms",
                    controller, action, status, elapsed);
                break;

            case LogLevel.Warning:
                _logger.LogWarning(
                    "{Controller}.{Action} retornou {Status} em {Elapsed}ms",
                    controller, action, status, elapsed);
                break;

            case LogLevel.Debug when status == 404:
                _logger.LogDebug(
                    "{Controller}.{Action} retornou NotFound em {Elapsed}ms",
                    controller, action, elapsed);
                break;

            case LogLevel.Debug:
                var args = context.ActionArguments;
                _logger.LogDebug(
                    "Entrando em {Controller}.{Action} com {@Args}",
                    controller, action, args);
                _logger.LogDebug(
                    "{Controller}.{Action} finalizou {Status} em {Elapsed}ms",
                    controller, action, status, elapsed);
                break;
        }
    }
}