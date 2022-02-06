using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CloudSeedApp;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace AppTests {

    [CollectionDefinition(nameof(IntegrationFixtureCollection))]
    public class IntegrationFixtureCollection : ICollectionFixture<IntegrationFixture> {}

    public class IntegrationFixture : IAsyncLifetime {
        // public CloudSeedAppDatabaseContext DbContext { get; private set; }

        public CloudSeedAppDatabaseContext DbContext = null!;
        public IServiceScopeFactory ScopeFactory => _scopeFactory;

        private IConfigurationRoot _configuration = null!;
        private IServiceScopeFactory _scopeFactory = null!;

        public IntegrationFixture() {
            this.DbContext = DatabaseFixture.CreateTestDatabaseContext();

            _configuration = TestConfiguration
                .GetIConfigurationRoot(Directory.GetCurrentDirectory());

            var webHostEnvironment = Mock.Of<IWebHostEnvironment>(
                w => w.EnvironmentName == "Development"
            );

            var startup = new Startup(
                webHostEnvironment,
                _configuration);

            var services = new ServiceCollection();
            services.AddSingleton(webHostEnvironment);
            services.AddLogging();

            startup.ConfigureServices(services);
            _scopeFactory = services.BuildServiceProvider()
                .GetRequiredService<IServiceScopeFactory>();
        }

        // public Task SendAsync(IRequest request)
        // {
        //     using var scope = _scopeFactory.CreateScope();
        //     var mediator = scope
        //         .ServiceProvider
        //         .GetRequiredService<IMediator>();

        //     return mediator.Send(request);
        // }

        public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            // using var scope = _scopeFactory.CreateScope();
            var scope = _scopeFactory.CreateAsyncScope();
            var mediator = scope
                .ServiceProvider
                .GetRequiredService<IMediator>();

            return mediator.Send(request);
        }

        public Task InitializeAsync() 
            => Task.CompletedTask;

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
