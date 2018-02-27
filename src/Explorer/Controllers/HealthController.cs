// -----------------------------------------------------------------------
// <copyright file="HealthController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Logic;
    using Logic.Office;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Models;
    using PartnerCenter.Models.Customers;
    using PartnerCenter.Models.Invoices;
    using PartnerCenter.Models.Subscriptions;
    using Providers;
    using Security;

    /// <summary>
    /// Handles request for the Health views.
    /// </summary>
    [AuthorizationFilter(Roles = UserRole.Partner)]
    public class HealthController : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HealthController"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        public HealthController(IExplorerProvider provider) : base(provider)
        {
        }

        /// <summary>
        /// Handles the index view request.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns>The HTML template for the index page.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// or
        /// <paramref name="subscriptionId"/> is empty or null.
        /// </exception>
        public async Task<ActionResult> Index(string customerId, string subscriptionId)
        {
            Customer customer;
            SubscriptionHealthModel healthModel;
            Subscription subscription;

            customerId.AssertNotEmpty(nameof(customerId));
            subscriptionId.AssertNotEmpty(nameof(subscriptionId));

            try
            {
                customer = await Provider.PartnerOperations.GetCustomerAsync(customerId).ConfigureAwait(false);
                subscription = await Provider.PartnerOperations.GetSubscriptionAsync(customerId, subscriptionId).ConfigureAwait(false);

                healthModel = new SubscriptionHealthModel
                {
                    CompanyName = customer.CompanyProfile.CompanyName,
                    CustomerId = customerId,
                    FriendlyName = subscription.FriendlyName,
                    SubscriptionId = subscriptionId,
                    ViewModel = (subscription.BillingType == BillingType.License) ? "Office" : "Azure"
                };

                if (subscription.BillingType == BillingType.Usage)
                {
                    healthModel.HealthEvents = await GetAzureSubscriptionHealthAsync(customerId, subscriptionId);
                }
                else
                {
                    healthModel.HealthEvents = await GetOfficeSubscriptionHealthAsync(customerId).ConfigureAwait(false);
                }

                return View(healthModel.ViewModel, healthModel);
            }
            finally
            {
                customer = null;
                subscription = null;
            }
        }

        /// <summary>
        /// Gets a list of health events for the specified Azure subscription.
        /// </summary>
        /// <param name="customerId">Identifier of the customer.</param>
        /// <param name="subscriptionId">Identifier of the subscription.</param>
        /// <returns>A list of health events for the Azure subscription.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// or
        /// <paramref name="subscriptionId"/> is empty or null.
        /// </exception>
        private async Task<List<IHealthEvent>> GetAzureSubscriptionHealthAsync(string customerId, string subscriptionId)
        {
            AuthenticationResult token;

            customerId.AssertNotEmpty(nameof(customerId));
            subscriptionId.AssertNotEmpty(nameof(subscriptionId));

            try
            {
                token = await Provider.AccessToken.GetAccessTokenAsync(
                    $"{Provider.Configuration.ActiveDirectoryEndpoint}/{customerId}",
                      Provider.Configuration.AzureResourceManagerEndpoint,
                    new ApplicationCredential
                    {
                        ApplicationId = Provider.Configuration.ApplicationId,
                        ApplicationSecret = Provider.Configuration.ApplicationSecret,
                        UseCache = true
                    },
                    Provider.AccessToken.UserAssertionToken).ConfigureAwait(false);

                // TODO -- Create a custom HttpService to query Azure health events.

                //using (Insights insights = new Insights(subscriptionId, token.AccessToken))
                //{
                //    return await insights.GetHealthEventsAsync().ConfigureAwait(false);
                //}

                return null;
            }
            finally
            {
                token = null;
            }
        }

        /// <summary>
        /// Gets a list of health events for the license based subscription.
        /// </summary>
        /// <param name="customerId">Identifier of the customer.</param>
        /// <returns>A list of health events.</returns>
        private async Task<List<IHealthEvent>> GetOfficeSubscriptionHealthAsync(string customerId)
        {
            AuthenticationResult token;
            ServiceCommunications comm;

            customerId.AssertNotEmpty(nameof(customerId));

            try
            {
                token = await Provider.AccessToken.GetAccessTokenAsync(
                    $"{Provider.Configuration.ActiveDirectoryEndpoint}/{customerId}",
                      Provider.Configuration.OfficeManagementEndpoint,
                    new ApplicationCredential
                    {
                        ApplicationId = Provider.Configuration.ApplicationId,
                        ApplicationSecret = Provider.Configuration.ApplicationSecret,
                        UseCache = true
                    }).ConfigureAwait(false);

                comm = new ServiceCommunications(Provider, token);
                return await comm.GetCurrentStatusAsync(customerId).ConfigureAwait(false);
            }
            finally
            {
                comm = null;
                token = null;
            }
        }
    }
}