using System;
using System.Collections.Generic;
using CloudSeedApp;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CloudSeedApp {
    public class CloudSeedAppDatabaseContext : DbContext
    {
        public DbSet<Checkout> Checkouts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ApplicationRole> Roles { get; set; }
        public DbSet<Sentinel> Sentinels { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<User> Users { get; set; }

        private readonly IDomainEventService _domainEventService;
        public CloudSeedAppDatabaseContext (
            DbContextOptions<CloudSeedAppDatabaseContext> options,
            IDomainEventService domainEventService)
            : base(options)
        {
            this._domainEventService = domainEventService;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken()) {

            /*
                * This DomainEvents strategy was pulled from several sources
                    * It is fast and relatively safe
                    * CleanArchitecture repo - https://github.com/jasontaylordev/CleanArchitecture/blob/413fb3a68a0467359967789e347507d7e84c48d4/src/Infrastructure/Persistence/ApplicationDbContext.cs
                    * And is advocated for by Microsoft - https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-events-design-implementation
            */
            var events = ChangeTracker.Entries<IHasDomainEvent>()
                .Select(x => x.Entity.DomainEvents)
                .SelectMany(x => x)
                .Where(domainEvent => !domainEvent.IsPublished)
                .ToArray();

            var result = await base.SaveChangesAsync(cancellationToken);

            await DispatchEvents(events);

            return result;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity => 
            {
                entity.Property(u => u.Id)
                .HasDefaultValueSql("uuid_generate_v1()");
            });

            modelBuilder.Entity<Checkout>()
                .Property(c => c.ID)
                .HasDefaultValueSql("uuid_generate_v1()");

            modelBuilder.Entity<Order>()
                .Property(c => c.ID)
                .HasDefaultValueSql("uuid_generate_v1()");
            
            modelBuilder.Entity<Order>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(o => o.UserId);

            modelBuilder.Entity<Sentinel>(entity => {
                // HAM - required to prevent primary key errors on 'DomainEvent'
                entity.Ignore(s => s.DomainEvents);
            });

            modelBuilder.Entity<Subscription>(entity => {
                entity.HasKey(s => new { s.UserID, s.ProductID });
                // entity.Property(s => s.UserID)

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(s => s.UserID);
            });
        }

        public void MarkDataAsModified<T>(T entry) {
            this.Entry(entry).State = EntityState.Modified;
        }

        private async Task DispatchEvents(DomainEvent[] events)
        {
            foreach (var @event in events)
            {
                @event.IsPublished = true;
                await _domainEventService.Publish(@event);
            }
        }
    }
}