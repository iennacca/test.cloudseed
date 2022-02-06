namespace CloudSeedApp;

public class SentinelCreatedEvent : DomainEvent {

    public Sentinel Sentinel { get; set; }

    public SentinelCreatedEvent(Sentinel sentinel) {
        this.Sentinel = sentinel;
    }
}