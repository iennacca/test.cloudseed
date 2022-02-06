using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentValidation;
using MediatR;

namespace CloudSeedApp;

public class CreateCheckoutCommandValidator : AbstractValidator<CreateCheckoutCommand>
{
    private IMediator _mediator;

    public CreateCheckoutCommandValidator(IMediator mediator)
    {
        this._mediator = mediator;

        RuleFor(v => v.UserId)
            .NotEmpty()
            .NotNull()
            .MustAsync(async (userId, cancellation) => await CheckUserExistsAsync(userId))
                .WithMessage("User must exist");
        
        RuleFor(v => v.Products)
            .MustAsync(async (products, cancellation) => await CheckAllProductsExistAsync(products))
                .WithMessage("Invalid Product IDs");
    }

    private async Task<bool> CheckUserExistsAsync(Guid userId) {
        var getUserQuery = new GetUserQuery { UserId = userId };
        var user = await this._mediator.Send(getUserQuery);

        return user is not null;
    }

    private async Task<bool> CheckAllProductsExistAsync(List<OrderProductItem> products) {
        var existTasks = products 
            .Select(p => {
                var query = new GetProductQuery { ProductId = p.ItemID};
                return this._mediator.Send(query);
            }).ToList();
        
        var results = await Task.WhenAll(existTasks);

        return results.All(r => r is not null);
    }
}