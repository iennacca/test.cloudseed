using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace CloudSeedApp
{
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly ILogger<CheckoutController> _logger;
        private CloudSeedAppDatabaseContext _dbContext;
        private ProductDataProvider _productDataProvider;
        private CheckoutProcessor _checkoutProcessor;
        private CheckoutDataProvider _checkoutDataProvider;
        private ConfigurationProvider _configurationProvider;
        private ICurrentUserService _currentUserService;
        private IMediator _mediator;
        private INowProvider _nowProvider;
        private readonly IOrderService _orderService;
        private StripeCheckoutProcessor _stripeCheckoutProcessor;
        private UserDataProvider _userDataProvider;

        public CheckoutController(
            ILogger<CheckoutController> logger,
            CloudSeedAppDatabaseContext dbContext,
            ProductDataProvider productDataProvider,
            CheckoutProcessor checkoutProcessor,
            CheckoutDataProvider checkoutDataProvider,
            ConfigurationProvider configurationProvider,
            ICurrentUserService currentUserService,
            IMediator mediator,
            INowProvider nowProvider,
            IOrderService orderService,
            StripeCheckoutProcessor stripeCheckoutProcessor,
            UserDataProvider userDataProvider)
        {
            _logger = logger;
            this._dbContext = dbContext;
            this._productDataProvider = productDataProvider;
            this._checkoutProcessor = checkoutProcessor;
            this._checkoutDataProvider = checkoutDataProvider;
            this._configurationProvider = configurationProvider;
            this._currentUserService = currentUserService;
            this._mediator = mediator;
            this._nowProvider = nowProvider;
            this._orderService = orderService;
            this._stripeCheckoutProcessor = stripeCheckoutProcessor;
            this._userDataProvider = userDataProvider;
        }

        [HttpPost]
        [Route("/checkout/create")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateCheckout(
            CheckoutApiMetadata metadata
        )
        {
            var userId = Guid.Parse(this._currentUserService.UserId);

            var getUserQuery = new GetUserQuery { UserId = userId };
            var user = await this._mediator.Send(getUserQuery);
            if(user is null) {
                throw new InvalidOperationException(
                    "No user!"
                );
            }

            var createCheckoutCommand = new CreateCheckoutCommand {
                UserId = userId,
                Products = metadata.Products.Select(
                    p => new OrderProductItem(
                        p.ItemID,
                        p.Quantity
                    )
                ).ToList()
            };
            var checkout = await this._mediator.Send(createCheckoutCommand);
            var checkoutUrl = await this._orderService
                .GetCheckoutPortalUrlForCheckoutAsync(checkout.ID);

            return Ok(
                new CheckoutApiCreationResponse{
                    CheckoutUrl = checkoutUrl
                }
            );
        }

        [HttpPost]
        [Route("/webhooks/stripe")]
        [AllowAnonymous]
        public async Task<IActionResult> StripeWebhook()
        {
            // All stripe event types - https://stripe.com/docs/api/events/types
            this._logger
                .LogInformation("hamy - stripe webhook");

            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var endpointSecret = this._configurationProvider.STRIPE_WEBHOOK_SECRET;
            try
            {
                // This will fail if the endpoint secret is incorrect
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    endpointSecret);

                // Checkouts - https://stripe.com/docs/api/events/types?lang=dotnet#event_types-checkout.session.completed
                // The object associated with this is a CheckoutSession object - https://stripe.com/docs/api/events/types?lang=dotnet#event_types-checkout.session.completed
                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                { 
                    // sent when user successfully goes through checkout flow - https://stackoverflow.com/questions/62882815/when-does-checkout-session-completed-trigger
                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                    if(session is null) {
                        throw new ArgumentNullException(
                            "Null session received"
                        );
                    }

                    await this._stripeCheckoutProcessor
                        .HandleCheckoutSessionCompleted(
                            session.Id,
                            session.CustomerId
                        );
                }
                else if (stripeEvent.Type == Events.InvoicePaid)
                {
                    var invoice = stripeEvent.Data.Object as Stripe.Invoice;

                    if(invoice is null) {
                        throw new InvalidOperationException(
                            "Invoice cannot be null"
                        );
                    }

                    var stripeProducts = invoice.Lines.Data.Select(
                        l => {
                           return new{
                                PriceId = l.Price.Id,
                                Quantity = l.Quantity ?? 1
                            };
                        }
                    );

                    var products = stripeProducts
                        .Select(p => {
                            this._productDataProvider.TryGetProductByStripePriceId(p.PriceId, out var product);
                            if(product is null) {
                                throw new InvalidDataException(
                                    $"Could not find corresponding product for priceId {p}"
                                );
                            }
                            return new OrderProductItem(
                                product.ID,
                                (int)p.Quantity
                            );
                        }).ToList();

                    await this._stripeCheckoutProcessor
                        .HandleInvoicePaidAsync(
                            invoice.CustomerId,
                            invoice.Id,
                            products
                        );
                }

                // Subscriptions - https://stripe.com/docs/billing/quickstart
                // if (stripeEvent.Type == Events.CustomerSubscriptionCreated)
                // {
                //     /*
                //         * Check to see if a user is related to the checkout
                //     */
                // }
                // else if (stripeEvent.Type == Events.CustomerSubscriptionUpdated)
                // {

                // }
                // else if (stripeEvent.Type == Events.CustomerSubscriptionDeleted)
                // {

                // }
                // ... handle other event types
                else
                {
                    this._logger.LogError($"Unhandled event type: {stripeEvent.Type}");
                }

                return Ok();
            }
            catch (StripeException e)
            {
                this._logger.LogError(e.Message);
                return BadRequest();
            }
        }
    }
}
