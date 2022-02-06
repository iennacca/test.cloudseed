using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CloudSeedApp;

public class GetUserSubscriptionsQuery : IRequest<IEnumerable<Subscription>> 
{ 
    public Guid UserId { get; set; }
}

public class GetUserSubscriptionsQueryHandler : IRequestHandler<GetUserSubscriptionsQuery, IEnumerable<Subscription>> {
    private readonly CloudSeedAppDatabaseContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetUserSubscriptionsQueryHandler(
        CloudSeedAppDatabaseContext context,
        ICurrentUserService currentUserService
    ) {
        this._context = context;
        this._currentUserService = currentUserService;
    }

    public async Task<IEnumerable<Subscription>> Handle(GetUserSubscriptionsQuery request, CancellationToken cancellationToken) {     
        return await this._context
            .Subscriptions
            .Where(s => s.UserID == request.UserId)
            .ToListAsync();
    }
}