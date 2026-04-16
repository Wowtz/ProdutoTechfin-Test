using MassTransit;
using Microsoft.Extensions.Logging;
using ProdutoTechfin.Domain.Events;

namespace ProdutoTechfin.Infrastructure.Messaging.Consumers
{
    public class ProductUpdatedEventConsumer : IConsumer<ProductUpdatedEvent>
    {
        private readonly ILogger<ProductUpdatedEventConsumer> _logger;

        public ProductUpdatedEventConsumer(ILogger<ProductUpdatedEventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ProductUpdatedEvent> context)
        {
            _logger.LogInformation(
                "Product updated event received. ProductId: {ProductId}, OccurredAt: {OccurredAt}",
                context.Message.Id,
                context.Message.OccurredAt);

            await Task.CompletedTask;
        }
    }
}
