using MassTransit;
using Microsoft.Extensions.Logging;
using ProdutoTechfin.Domain.Events;

namespace ProdutoTechfin.Infrastructure.Messaging.Consumers
{
    public class ProductDeletedEventConsumer : IConsumer<ProductDeletedEvent>
    {
        private readonly ILogger<ProductDeletedEventConsumer> _logger;

        public ProductDeletedEventConsumer(ILogger<ProductDeletedEventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ProductDeletedEvent> context)
        {
            _logger.LogInformation(
                "Product deleted event received. ProductId: {ProductId}, OccurredAt: {OccurredAt}",
                context.Message.Id,
                context.Message.OccurredAt);

            await Task.CompletedTask;
        }
    }
}
