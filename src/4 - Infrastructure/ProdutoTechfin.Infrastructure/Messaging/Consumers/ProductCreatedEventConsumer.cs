using MassTransit;
using Microsoft.Extensions.Logging;
using ProdutoTechfin.Domain.Events;

namespace ProdutoTechfin.Infrastructure.Messaging.Consumers
{
    public class ProductCreatedEventConsumer : IConsumer<ProductCreatedEvent>
    {
        private readonly ILogger<ProductCreatedEventConsumer> _logger;

        public ProductCreatedEventConsumer(ILogger<ProductCreatedEventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ProductCreatedEvent> context)
        {
            _logger.LogInformation(
                "Product created event received. ProductId: {ProductId}, OccurredAt: {OccurredAt}",
                context.Message.Id,
                context.Message.OccurredAt);

            await Task.CompletedTask;
        }
    }
}
