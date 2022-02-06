using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CloudSeedApp {
    public class GetOrderManagementUrlQuery : IRequest<string> { 
        public Guid UserId { get; set; }
    }

    public class GetOrderManagementUrlQueryHandler : IRequestHandler<GetOrderManagementUrlQuery, string> {
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;
        private readonly IOrderService _orderService;

        public GetOrderManagementUrlQueryHandler(
            ICurrentUserService currentUserService,
            IMediator mediator,
            IOrderService orderService
        ) {
            this._currentUserService = currentUserService;
            this._mediator = mediator;
            this._orderService = orderService;
        }

        public async Task<string> Handle(GetOrderManagementUrlQuery request, CancellationToken cancellationToken) {
            if(Guid.Parse(this._currentUserService.UserId) != request.UserId) {
                throw new ForbiddenAccessException();
            }

            var getUserQuery = new GetUserQuery { UserId = request.UserId };
            var user = await this._mediator.Send(getUserQuery);
            if(user is null) {
                throw new InvalidOperationException(
                    $"User does not exist: {request.UserId}"
                );
            }

            return await this._orderService.GetOrderManagementPortalUrlForUserAsync(user.Id);
        }
    }
}