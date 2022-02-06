using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;
using Microsoft.EntityFrameworkCore;

using CloudSeedApp;

namespace AppTests;

[Collection(nameof(DatabaseCollectionFixture))]
public class CreateSentinelCommandTests : DatabaseTest
{
    public CreateSentinelCommandTests(DatabaseFixture fixture) : base(fixture) {

    }

    [Fact]
    public async Task TestCreateSentinelCommand_Successful() {
        var expectedId = Guid.NewGuid().ToString();
        var command = new CreateSentinelCommand { Name = expectedId };

        var handler = new CreateSentinelCommandHandler(this.DbContext);
        var createdSentinel = await handler.Handle(
            command,
            new System.Threading.CancellationToken()
        );
        Assert.Equal(expectedId, createdSentinel.Name);

        var count = await this.DbContext
            .Sentinels
            .CountAsync();
        Assert.True(count > 0);

        // Confirm in db
        await this.DbContext
            .Sentinels
            .SingleAsync(s => s.Name == expectedId);
    }
}