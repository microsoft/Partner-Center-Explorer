// -----------------------------------------------------------------------
// <copyright file="UsageController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Logic;
    using Models;
    using PartnerCenter.Models.Customers;
    using PartnerCenter.Models.Subscriptions;
    using Providers;
    using Security;

    /// <summary>
    /// Controller for all Usage views.
    /// </summary>
    [AuthorizationFilter(Roles = UserRole.Partner)]
    public class UsageController : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsageController"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        public UsageController(IExplorerProvider provider) : base(provider)
        {
        }

        /// <summary>
        /// Views the usage.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns>The HTML template for the view usage page.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="customerId"/> is empty or null.
        /// or
        /// <paramref name="subscriptionId"/> is empty or null.
        /// </exception>
        public async Task<ActionResult> ViewUsage(string customerId, string subscriptionId)
        {
            Customer customer;
            Subscription subscription;

            customerId.AssertNotEmpty(nameof(customerId));
            subscriptionId.AssertNotEmpty(nameof(subscriptionId));

            try
            {
                customer = await Provider.PartnerOperations.GetCustomerAsync(customerId).ConfigureAwait(false);
                subscription = await Provider.PartnerOperations.GetSubscriptionAsync(customerId, subscriptionId).ConfigureAwait(false);

                UsageModel usageModel = new UsageModel()
                {
                    CompanyName = customer.CompanyProfile.CompanyName,
                    CustomerId = customerId,
                    SubscriptionId = subscriptionId,
                    SubscriptionFriendlyName = subscription.FriendlyName,
                    Usage = await Provider.PartnerOperations
                        .GetSubscriptionUsageAsync(customerId, subscriptionId, DateTime.Now.AddMonths(-1), DateTime.Now).ConfigureAwait(false)
                };

                return View(usageModel);
            }
            finally
            {
                customer = null;
                subscription = null;
            }
        }
    }
}