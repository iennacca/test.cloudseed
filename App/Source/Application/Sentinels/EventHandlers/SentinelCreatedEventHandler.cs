using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

namespace CloudSeedApp;

public class SentinelCreatedEventHandler : INotificationHandler<DomainEventNotification<SentinelCreatedEvent>> {
    private readonly IMediator _mediator;

    public SentinelCreatedEventHandler(IMediator mediator) {
        this._mediator = mediator;
    }

    public async Task Handle(DomainEventNotification<SentinelCreatedEvent> notification, CancellationToken cancellationToken) {
        var domainEvent = notification.DomainEvent;

        var allSentinels = await this._mediator.Send(new GetSentinelsQuery());
        if(allSentinels.Count() > 100) {
            await this._mediator.Send(new DeleteAllSentinelsCommand());
        }
    }
}