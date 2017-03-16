// -----------------------------------------------------------------------
// <copyright file="HealthController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Logic;
    using Logic.Azure;
    using Logic.Office;
    using Models;
    using PartnerCenter.Models.Customers;
    using PartnerCenter.Models.Invoices;
    using PartnerCenter.Models.Subscriptions;
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
        public HealthController(IExplorerService service) : base(service)
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
                customer = await this.Service.PartnerCenter.Customers.ById(customerId).GetAsync();
                subscription = await this.Service.PartnerCenter.Customers.ById(customerId).Subscriptions.ById(subscriptionId).GetAsync();

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
                    healthModel.HealthEvents = await this.GetAzureSubscriptionHealthAsync(customerId, subscriptionId);
                }
                else
                {
                    healthModel.HealthEvents = await this.GetOfficeSubscriptionHealthAsync(customerId);
                }

                return this.View(healthModel.ViewModel, healthModel);
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
            AuthenticationToken token;

            customerId.AssertNotEmpty(nameof(customerId));
            subscriptionId.AssertNotEmpty(nameof(subscriptionId));

            try
            {
                token = await this.Service.TokenManagement.GetAppPlusUserTokenAsync(
                    $"{this.Service.Configuration.ActiveDirectoryEndpoint}/{customerId}",
                    this.Service.Configuration.AzureResourceManagerEndpoint);

                using (Insights insights = new Insights(subscriptionId, token.Token))
                {
                    return await insights.GetHealthEventsAsync();
                }
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
            AuthenticationToken token;
            ServiceCommunications comm;

            customerId.AssertNotEmpty(nameof(customerId));

            try
            {
                token = await this.Service.TokenManagement.GetAppOnlyTokenAsync(
                    $"{this.Service.Configuration.ActiveDirectoryEndpoint}/{customerId}",
                    this.Service.Configuration.OfficeManagementEndpoint);

                comm = new ServiceCommunications(this.Service, token);
                return await comm.GetCurrentStatusAsync(customerId);
            }
            finally
            {
                comm = null;
                token = null;
            }
        }
    }
}