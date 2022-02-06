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
public class GetUserQueryTests
{
    private IntegrationFixture _fixture;

    public GetUserQueryTests(IntegrationFixture fixture) 
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task TestGetUserQuery() {
        var user = await TestDataPopulationUtilities
            .CreateUserAsync(this._fixture.DbContext);

        var query = new GetUserQuery { UserId = user.Id };
        var actualUser = await this._fixture.SendAsync(query);

        Assert.NotNull(actualUser);
        Assert.Equal(user.Email, actualUser.Email);
    }
}