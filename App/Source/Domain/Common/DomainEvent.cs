using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudSeedApp;

public interface IHasDomainEvent
{
    public List<DomainEvent> DomainEvents { get; set; }
}

public abstract class EntityWithDomainEvent : IHasDomainEvent {

    [NotMapped]
    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
}

public abstract class DomainEvent
{
    protected DomainEvent()
    {
        DateOccurred = DateTimeOffset.UtcNow;
    }
    [NotMapped]
    public bool IsPublished { get; set; }
    [NotMapped]
    public DateTimeOffset DateOccurred { get; protected set; } = DateTimeOffset.UtcNow;
}