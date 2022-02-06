using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CloudSeedApp {
    public class CreateSentinelCommand : IRequest<Sentinel> {
        public string Name { get; set; }
    }

    public class CreateSentinelCommandHandler : IRequestHandler<CreateSentinelCommand, Sentinel> {
        private readonly CloudSeedAppDatabaseContext _context;

        public CreateSentinelCommandHandler(
            CloudSeedAppDatabaseContext context
        ) {
            this._context = context;
        }

        public async Task<Sentinel> Handle(CreateSentinelCommand request, CancellationToken cancellationToken) {
            var newSentinel = new Sentinel {
                Name = request.Name
            };
            newSentinel
                .DomainEvents
                .Add(new SentinelCreatedEvent(newSentinel));

            this._context
                .Sentinels
                .Add(newSentinel);
            await this._context.SaveChangesAsync();

            return newSentinel;
        }
    }
}