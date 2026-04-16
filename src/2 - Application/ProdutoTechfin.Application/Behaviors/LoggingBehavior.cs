using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ProdutoTechfin.CrossCutting.Logging;

namespace ProdutoTechfin.Application.Behaviors
{
    [ExcludeFromCodeCoverage]
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var stopwatch = Stopwatch.StartNew();

            using var scope = _logger.BeginOperationScope(requestName);

            _logger.LogInformation(
                "Handling {RequestName} with payload {@Request}",
                requestName, request);

            try
            {
                var response = await next();
                stopwatch.Stop();

                _logger.LogOperationResult(requestName, success: true, elapsedMs: stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(ex,
                    "Unhandled exception in {RequestName}",
                    requestName);

                _logger.LogOperationResult(
                    requestName,
                    success: false,
                    elapsedMs: stopwatch.ElapsedMilliseconds,
                    detail: ex.Message);

                throw;
            }
        }
    }
}
