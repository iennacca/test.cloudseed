using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CloudSeedApp {
    public class GetProductQuery : IRequest<Product?> { 
        public string ProductId { get; set; }
    }

    public class GetProductQueryHandler : IRequestHandler<GetProductQuery, Product?> {
        private readonly ProductDataProvider _productProvider;

        public GetProductQueryHandler(
            ProductDataProvider productProvider
        ) {
            this._productProvider = productProvider;
        }

        public async Task<Product?> Handle(GetProductQuery request, CancellationToken cancellationToken) {
            
            var _ = this._productProvider
                .TryGetProductByID(request.ProductId, out var product);
            return product;
        }
    }
}