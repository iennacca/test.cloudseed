using System;
using System.IO;
using System.Threading.Tasks;
using CloudSeedApp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;


namespace AppTests;

public static class TestDataPopulationUtilities
{

    public static async Task<User> CreateUserAsync(CloudSeedAppDatabaseContext dbContext)
    {
        var email = Guid.NewGuid().ToString();
        var user = new User(
            email,
            new User.UserData()
        );
        await dbContext
            .Users
            .AddAsync(user);
        await dbContext.SaveChangesAsync();
        return user;
    }

    #region Checkouts
    public static async Task<Checkout> CreateCheckoutAsync(
        CloudSeedAppDatabaseContext dbContext)
    {
        var nowProvider = new UtcNowProvider();
        var nowDateTimeOffset = nowProvider.GetNowDateTimeOffset();
        var twoDaysFromNowDateTimeOffset = nowDateTimeOffset.AddDays(2);

        var checkout = new Checkout(
            nowDateTimeOffset,
            twoDaysFromNowDateTimeOffset,
            new Checkout.CheckoutData {
                StripeCheckoutID = Guid.NewGuid().ToString()
            }
        );
        await dbContext
            .Checkouts
            .AddAsync(checkout);
        await dbContext.SaveChangesAsync();

        return checkout;
    }

    public static CheckoutDataProvider CreateCheckoutDataProvider(CloudSeedAppDatabaseContext dbContext) {
        return new CheckoutDataProvider(
            dbContext,
            TestDataPopulationUtilities.GetMemoryCache<Checkout>()
        );
    }
    #endregion Checkouts

    public static async Task<Order> CreateOrderAsync(
        CloudSeedAppDatabaseContext dbContext,
        Guid userID,
        DateTimeOffset dateTimeOffset
    )
    {
        var order = new Order(
            userID,
            dateTimeOffset,
            new Order.OrderData()
        );
        await dbContext
            .Orders
            .AddAsync(order);
        await dbContext.SaveChangesAsync();
        return order;
    }

    #region Products
    public static ProductDataProvider CreateProductDataProvider() {
        return new ProductDataProvider(
            ProductConfiguration.GetTestProducts()
        );
    }
    #endregion Products

    #region Subscriptions
    public static async Task<Subscription> CreateSubscriptionAsync(
        CloudSeedAppDatabaseContext dbContext,
        User? user = null,
        Product? product = null,
        DateTimeOffset? expirationTimestamp = null
    ) {
        var nowProvider = new UtcNowProvider();
        
        User subscriptionUser = user;
        if(subscriptionUser is null) {
            subscriptionUser = await CreateUserAsync(dbContext);
        }

        var subscription = new Subscription(
            subscriptionUser.Id,
            product?.ID ?? ProductConfiguration.GetTestProducts()[0].ID,
            nowProvider.GetNowDateTimeOffset(),
            expirationTimestamp ?? nowProvider.GetNowDateTimeOffset(),
            new Subscription.SubscriptionData()
        );
        await dbContext
            .Subscriptions
            .AddAsync(subscription);
        await dbContext.SaveChangesAsync();
        return subscription;
    }

    public static SubscriptionDataProvider CreateSubscriptionDataProvider(CloudSeedAppDatabaseContext dbContext) {
        return new SubscriptionDataProvider(
            dbContext,
            new UtcNowProvider(),
            CreateUserDataProvider(dbContext)
        );
    }
    #endregion Subscriptions

    public static UserDataProvider CreateUserDataProvider(CloudSeedAppDatabaseContext dbContext) {
        return new UserDataProvider(
            dbContext,
            TestDataPopulationUtilities.GetMemoryCache<User>()
        );
    }

    public static ConfigurationProvider GetConfigurationProvider()
    {
        var webHostEnvironment = Mock.Of<IWebHostEnvironment>(
            w => w.EnvironmentName == "Development"
        );
        return new ConfigurationProvider(
            TestConfiguration.GetIConfigurationRoot(
                Directory.GetCurrentDirectory()
            ),
            webHostEnvironment);
    }

    public static TSimpleMemoryCache<T> GetMemoryCache<T>() {
        return new TSimpleMemoryCache<T>(
            new SimpleMemoryCacheOptions(
                100,
                TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(10)
            )
        );
    }

    public static string CreateNewEmail() {
        return $"{Guid.NewGuid().ToString()}@thisisatest.xyz";
    }
}
