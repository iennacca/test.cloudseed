using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CloudSeedApp {
    public class CreateCheckoutCommand : IRequest<Checkout> {
        public Guid UserId { get; set; }

        public List<OrderProductItem> Products { get; set; }
        // userId,
        //     List<OrderProductItem> products
    }

    public class CreateCheckoutCommandHandler : IRequestHandler<CreateCheckoutCommand, Checkout> {
        private readonly CloudSeedAppDatabaseContext _context;
        private readonly INowProvider _nowProvider;

        public CreateCheckoutCommandHandler(
            CloudSeedAppDatabaseContext context,
            INowProvider nowProvider
        ) {
            this._context = context;
            this._nowProvider = nowProvider;
        }

        public async Task<Checkout> Handle(CreateCheckoutCommand request, CancellationToken cancellationToken) {
            var now = this._nowProvider.GetNowDateTimeOffset();
            var checkout = new Checkout(
                now,
                now.AddDays(7),
                new Checkout.CheckoutData {
                    UserID = request.UserId,
                    Products = request.Products
                }
            );
            await this._context
                .Checkouts
                .AddAsync(checkout);
            await this._context.SaveChangesAsync();
            
            return checkout;
        }
    }
}