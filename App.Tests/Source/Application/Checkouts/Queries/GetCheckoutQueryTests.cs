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
public class GetCheckoutQueryTests
{
    private IntegrationFixture _fixture;

    public GetCheckoutQueryTests(IntegrationFixture fixture) 
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task TestGetCheckoutQuery() {
        var checkout = await TestDataPopulationUtilities
            .CreateCheckoutAsync(this._fixture.DbContext);

        var query = new GetCheckoutQuery { CheckoutId = checkout.ID.ToString() };
        var actualCheckout = await this._fixture.SendAsync(query);

        Assert.NotNull(actualCheckout);
        Assert.Equal(checkout.Data.UserID, actualCheckout.Data.UserID);
    }

    [Fact]
    public async Task TestGetCheckoutQueryValidatesEmptyCheckoutId() {
        var checkout = await TestDataPopulationUtilities
            .CreateCheckoutAsync(this._fixture.DbContext);

        var query = new GetCheckoutQuery { CheckoutId = string.Empty };

        #pragma warning disable CS4014
        await Assert.ThrowsAsync<ValidationException>(
            () => this._fixture.SendAsync(query)
        );
        #pragma warning restore CS4014
    }
}