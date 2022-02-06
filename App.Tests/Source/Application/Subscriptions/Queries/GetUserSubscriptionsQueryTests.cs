using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CloudSeedApp;
using Xunit;
using Xunit.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AppTests;

[Collection(nameof(IntegrationFixtureCollection))]
public class GetUserSubscriptionsQueryTests
{
    private IntegrationFixture _fixture;

    public GetUserSubscriptionsQueryTests(IntegrationFixture fixture) 
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task TestGetSubscriptionsForUser() {
        var subscription = await TestDataPopulationUtilities
            .CreateSubscriptionAsync(this._fixture.DbContext);

        var query = new GetUserSubscriptionsQuery { UserId = subscription.UserID };

        var actualSubscriptionListMaybe = await this._fixture.SendAsync(query);

        Assert.NotNull(actualSubscriptionListMaybe);

        var actualSubscription = actualSubscriptionListMaybe
            .Single();

        Assert.NotNull(actualSubscription);
        Assert.Equal(subscription.UserID, actualSubscription.UserID);
        Assert.Equal(subscription.ProductID, actualSubscription.ProductID);
    }
}