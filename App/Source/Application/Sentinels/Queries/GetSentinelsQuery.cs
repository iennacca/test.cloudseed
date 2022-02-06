using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CloudSeedApp {
    public class GetSentinelsQuery : IRequest<IEnumerable<Sentinel>> { }

    public class GetSentinelsQueryHandler : IRequestHandler<GetSentinelsQuery, IEnumerable<Sentinel>> {
        private readonly CloudSeedAppDatabaseContext _context;

        public GetSentinelsQueryHandler(
            CloudSeedAppDatabaseContext context
        ) {
            this._context = context;
        }

        public async Task<IEnumerable<Sentinel>> Handle(GetSentinelsQuery request, CancellationToken cancellationToken) {
            var allSentinels = await this._context
                .Sentinels
                .ToListAsync();
            return allSentinels;
        }
    }
}