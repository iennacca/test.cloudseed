using Xunit;

namespace AppTests {

    [CollectionDefinition(nameof(DatabaseCollectionFixture))]
    public class DatabaseCollectionFixture : ICollectionFixture<DatabaseFixture> {

    }
}
