using System;
using System.Threading;
using System.Threading.Tasks;

namespace CloudSeedApp;

public interface IOrderService {

    public Task<string> GetCheckoutPortalUrlForCheckoutAsync(Guid checkoutId);

    public Task<string> GetOrderManagementPortalUrlForUserAsync(Guid userId);
}