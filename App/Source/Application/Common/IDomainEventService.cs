using System.Threading.Tasks;

namespace CloudSeedApp;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}