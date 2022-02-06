using System;
using CloudSeedApp;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Xunit;

namespace AppTests {
    public abstract class DatabaseTest {

        public CloudSeedAppDatabaseContext DbContext { get; }

        protected DatabaseTest(DatabaseFixture databaseFixture) {
            DbContext = databaseFixture.DbContext;
        }
    }
}
