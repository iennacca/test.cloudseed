using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CloudSeedApp {
    public class GetCheckoutQuery : IRequest<Checkout> { 
        public string CheckoutId { get; set; }
    }

    public class GetCheckoutQueryHandler : IRequestHandler<GetCheckoutQuery, Checkout> {
        private readonly CloudSeedAppDatabaseContext _context;

        public GetCheckoutQueryHandler(
            CloudSeedAppDatabaseContext context
        ) {
            this._context = context;
        }

        public async Task<Checkout> Handle(GetCheckoutQuery request, CancellationToken cancellationToken) {
            var checkout = await this._context
                .Checkouts
                .SingleAsync(c => c.ID == Guid.Parse(request.CheckoutId));
            return checkout;
        }
    }
}