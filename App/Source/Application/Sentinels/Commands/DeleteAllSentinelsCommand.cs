using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CloudSeedApp {
    public class DeleteAllSentinelsCommand : IRequest<int> { }

    public class DeleteAllSentinelsCommandHandler : IRequestHandler<DeleteAllSentinelsCommand, int> {
        private readonly CloudSeedAppDatabaseContext _context;

        public DeleteAllSentinelsCommandHandler(
            CloudSeedAppDatabaseContext context
        ) {
            this._context = context;
        }

        public async Task<int> Handle(DeleteAllSentinelsCommand request, CancellationToken cancellationToken) {
            
            var count = await this._context
                .Sentinels
                .CountAsync();

            this._context
                .Sentinels
                .RemoveRange(this._context.Sentinels);
            await this._context.SaveChangesAsync();
            return count;
        }
    }
}