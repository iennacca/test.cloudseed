using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CloudSeedApp;

[ApiController]
public class SubscriptionsController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;

    public SubscriptionsController(
        ICurrentUserService currentUserService,
        IMediator mediator
    )
    {
        this._currentUserService = currentUserService;
        this._mediator = mediator;
    }

    [HttpGet]
    [Route("/users/{userId}/subscriptions")]
    public async Task<ActionResult<List<SubscriptionDto>>> GetSubscriptionsForUser(string userId)
    {
        if(userId != this._currentUserService.UserId) {
            throw new ForbiddenAccessException();
        }

        var query = new GetUserSubscriptionsQuery { UserId = Guid.Parse(userId) };
        var subscriptions = await this._mediator.Send(query);

        if (subscriptions is null)
        {
            return Ok(
                new List<SubscriptionDto>()
            );
        }

        var subscriptionDtos = subscriptions
            .Select(s =>
            {
                return new SubscriptionDto
                {
                    ProductId = s.ProductID,
                    ExpirationTimestamp = s.ExpirationTimestamp
                };
            }).ToList();

        return Ok(
            subscriptionDtos
        );
    }

    [HttpGet]
    [Route("/users/{userId}/subscriptions/manage")]
    public async Task<ActionResult<List<OrderManagementDto>>> GetSubscriptionManagementUrlForUser(string userId)
    {
        if(userId != this._currentUserService.UserId) {
            throw new ForbiddenAccessException();
        }

        var getOrderManagementPortalUrlQuery = new GetOrderManagementUrlQuery { UserId = Guid.Parse(userId) };
        var orderManagementPortalUrl = await this._mediator.Send(getOrderManagementPortalUrlQuery);

        return Ok(
            new OrderManagementDto {
                Url = orderManagementPortalUrl
            }
        );
    }
}
