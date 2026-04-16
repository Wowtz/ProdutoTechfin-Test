using MassTransit;
using Microsoft.Extensions.Logging;
using ProdutoTechfin.Application.Interfaces.Events;
using ProdutoTechfin.Domain.Events;

namespace ProdutoTechfin.Infrastructure.Messaging
{
    public class ProductEventPublisher : IProductEventPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<ProductEventPublisher> _logger;

        public ProductEventPublisher(
            IPublishEndpoint publishEndpoint,
            ILogger<ProductEventPublisher> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task PublishAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            var eventName = domainEvent.GetType().Name;

            try
            {
                _logger.LogInformation(
                    "Publishing domain event. Event: {EventName}, AggregateId: {AggregateId}, OccurredAt: {OccurredAt}",
                    eventName,
                    domainEvent.Id,
                    domainEvent.OccurredAt);

                await _publishEndpoint.Publish(domainEvent, domainEvent.GetType(), cancellationToken);

                _logger.LogInformation(
                    "Domain event published successfully. Event: {EventName}, AggregateId: {AggregateId}",
                    eventName,
                    domainEvent.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to publish domain event. Event: {EventName}, AggregateId: {AggregateId}",
                    eventName,
                    domainEvent.Id);

                throw;
            }
        }
    }
}
