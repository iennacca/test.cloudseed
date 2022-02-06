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
public class CreateCheckoutCommandTests
{
    private IntegrationFixture _fixture;

    public CreateCheckoutCommandTests(IntegrationFixture fixture) 
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task TestCreateCheckoutCommand() {
        var user = await TestDataPopulationUtilities 
            .CreateUserAsync(this._fixture.DbContext);
        var testProduct = ProductConfiguration
            .GetTestProducts()[0];

        var command = new CreateCheckoutCommand {
            UserId = user.Id,
            Products = new List<OrderProductItem> {
                new OrderProductItem(
                    testProduct.ID,
                    1
                )
            }
        };
        var checkout = await this._fixture.SendAsync(command);

        Assert.NotNull(checkout);

        var actualCheckout = await this._fixture
            .DbContext
            .Checkouts
            .SingleAsync(c => c.ID == checkout.ID);

        Assert.Equal(user.Id, actualCheckout.Data.UserID);
        Assert.Equal(testProduct.ID, actualCheckout.Data.Products[0].ItemID);

        // throw new NotImplementedException();
    }

    [Theory]
    // [InlineData("")]
    [InlineData(null)]
    #pragma warning disable CS1998
    public async Task TestCreateCheckoutCommandValidatesEmptyUserId(
    #pragma warning restore CS1998
        Guid? userId
    ) {
        Assert.Throws<InvalidOperationException>(
            () => new CreateCheckoutCommand { UserId = (Guid)userId }
        );
    }

    [Fact]
    public async Task TestCreateCheckoutCommandValidatesUserIdExists() {
        var nonexistentUserId = Guid.NewGuid();
        var command = new CreateCheckoutCommand { 
            UserId = nonexistentUserId,
            Products = new List<OrderProductItem>()
        };

        #pragma warning disable CS4014
        await Assert.ThrowsAsync<ValidationException>(
            () => this._fixture.SendAsync(command)
        );
        #pragma warning restore CS4014
    }

    [Fact]
    public async Task TestCreateCheckoutCommandValidatesProductsExist() {
        var user = await TestDataPopulationUtilities
            .CreateUserAsync(this._fixture.DbContext);

        var nonexistingProductId = "idonotexist";
        var command = new CreateCheckoutCommand {
            UserId = user.Id,
            Products = new List<OrderProductItem> {
                new OrderProductItem (
                    nonexistingProductId,
                    1
                )
            }
        };

        await Assert.ThrowsAsync<ValidationException>(
            () => this._fixture.SendAsync(command)
        );
    }
}