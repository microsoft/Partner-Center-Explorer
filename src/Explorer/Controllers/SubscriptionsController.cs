// -----------------------------------------------------------------------
// <copyright file="SubscriptionsController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Logic;
    using Models;
    using PartnerCenter.Models.Customers;
    using PartnerCenter.Models.Invoices;
    using PartnerCenter.Models.Orders;
    using PartnerCenter.Models.Subscriptions;
    using Providers;
    using Security;

    /// <summary>
    /// Handles request for the Subscriptions views.
    /// </summary>
    [AuthorizationFilter(Roles = UserRoles.Partner)]
    public class SubscriptionsController : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionsController"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        public SubscriptionsController(IExplorerProvider provider) : base(provider)
        {
        }

        /// <summary>
        /// Handles the HTTP GET request for the Create partial view.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>The HTML template for the subscription creation view.</returns>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="customerId"/> is empty or null. 
        /// </exception>
        [HttpGet]
        public PartialViewResult Create(string customerId)
        {
            customerId.AssertNotEmpty(nameof(customerId));

            NewSubscriptionModel newSubscriptionModel = new NewSubscriptionModel()
            {
                CustomerId = customerId
            };

            return PartialView(newSubscriptionModel);
        }

        /// <summary>
        /// Creates the order specified in the instance of <see cref="NewSubscriptionModel"/>.
        /// </summary>
        /// <param name="model">An aptly populated instance of <see cref="NewSubscriptionModel"/>.</param>
        /// <returns>A collection of subscriptions that belong to the customer.</returns>
        [HttpPost]
        public async Task<PartialViewResult> Create(NewSubscriptionModel model)
        {
            Order newOrder;
            SubscriptionsModel subscriptionsModel;

            try
            {
                newOrder = new Order()
                {
                    LineItems = model.LineItems,
                    ReferenceCustomerId = model.CustomerId
                };

                newOrder = await Provider.PartnerOperations.CreateOrderAsync(model.CustomerId, newOrder).ConfigureAwait(false);
                subscriptionsModel = await GetSubscriptionsAsync(model.CustomerId).ConfigureAwait(false);

                return PartialView("List", subscriptionsModel);
            }
            finally
            {
                newOrder = null;
            }
        }

        /// <summary>
        /// Edits the subscription represented by the instance of <see cref="SubscriptionModel"/>.
        /// </summary>
        /// <param name="model">An aptly populated instance of <see cref="SubscriptionModel"/>.</param>
        /// <returns>A HTTP status code of OK if the edit was successful.</returns>
        [HttpPost]
        public async Task<HttpResponseMessage> Edit(SubscriptionModel model)
        {
            Subscription subscription;

            try
            {
                subscription = await Provider.PartnerOperations.GetSubscriptionAsync(model.CustomerId, model.Id).ConfigureAwait(false);

                subscription.FriendlyName = model.FriendlyName;
                subscription.Status = model.Status;
                subscription.Quantity = model.Quantity;

                await Provider.PartnerOperations.UpdateSubscriptionAsync(model.CustomerId, subscription).ConfigureAwait(false);

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            finally
            {
                subscription = null;
            }
        }

        /// <summary>
        /// Lists all of the subscriptions owned by the specified customer identifier.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>The HTML template for the list view.</returns>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// </exception>
        public async Task<PartialViewResult> List(string customerId)
        {
            SubscriptionsModel subscriptionsModel;

            customerId.AssertNotEmpty(nameof(customerId));

            try
            {
                subscriptionsModel = await GetSubscriptionsAsync(customerId).ConfigureAwait(false);
                return PartialView(subscriptionsModel);
            }
            finally
            {
                subscriptionsModel = null;
            }
        }

        /// <summary>
        /// Handles the Show view request.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns>Returns either the Azure or Office subscription view.</returns>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// or
        /// <paramref name="subscriptionId"/> is empty or null.
        /// </exception>
        public async Task<ActionResult> Show(string customerId, string subscriptionId)
        {
            Customer customer;
            Subscription subscription;
            SubscriptionModel model;

            customerId.AssertNotEmpty(nameof(customerId));
            subscriptionId.AssertNotEmpty(nameof(subscriptionId));

            try
            {
                customer = await Provider.PartnerOperations.GetCustomerAsync(customerId).ConfigureAwait(false);
                subscription = await Provider.PartnerOperations.GetSubscriptionAsync(customerId, subscriptionId).ConfigureAwait(false);

                model = new SubscriptionModel()
                {
                    AutoRenewEnabled = subscription.AutoRenewEnabled,
                    BillingType = subscription.BillingType,
                    CommitmentEndDate = subscription.CommitmentEndDate,
                    CompanyName = customer.CompanyProfile.CompanyName,
                    CreationDate = subscription.CreationDate,
                    CustomerId = customerId,
                    EffectiveStartDate = subscription.EffectiveStartDate,
                    FriendlyName = subscription.FriendlyName,
                    Id = subscription.Id,
                    OfferId = subscription.OfferId,
                    OfferName = subscription.OfferName,
                    ParentSubscriptionId = subscription.ParentSubscriptionId,
                    PartnerId = subscription.PartnerId,
                    Quantity = subscription.Quantity,
                    Status = subscription.Status,
                    SuspensionReasons = subscription.SuspensionReasons,
                    UnitType = subscription.UnitType,
                    ViewModel = (subscription.BillingType == BillingType.License) ? "Office" : "Azure"
                };

                return View(model.ViewModel, model);
            }
            finally
            {
                customer = null;
                subscription = null;
            }
        }

        /// <summary>
        /// Gets the subscriptions for the specified customer.
        /// </summary>
        /// <param name="customerId">Identifier of the customer.</param>
        /// <returns>An instance of <see cref="SubscriptionsModel"/> that contains the subscriptions for the customer.</returns>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// </exception>
        private async Task<SubscriptionsModel> GetSubscriptionsAsync(string customerId)
        {
            List<Subscription> subscriptions;
            SubscriptionsModel subscriptionsModel;

            customerId.AssertNotEmpty(nameof(customerId));

            try
            {
                subscriptions = await Provider.PartnerOperations.GetSubscriptionsAsync(customerId).ConfigureAwait(false);
                subscriptionsModel = new SubscriptionsModel();

                foreach (Subscription s in subscriptions.Where(x => x.Status != SubscriptionStatus.Deleted))
                {
                    subscriptionsModel.Subscriptions.Add(new SubscriptionModel()
                    {
                        AutoRenewEnabled = s.AutoRenewEnabled,
                        BillingType = s.BillingType,
                        CommitmentEndDate = s.CommitmentEndDate,
                        CreationDate = s.CreationDate,
                        CustomerId = customerId,
                        EffectiveStartDate = s.EffectiveStartDate,
                        FriendlyName = s.FriendlyName,
                        Id = s.Id,
                        OfferId = s.OfferId,
                        OfferName = s.OfferName,
                        ParentSubscriptionId = s.ParentSubscriptionId,
                        PartnerId = s.PartnerId,
                        Quantity = s.Quantity,
                        Status = s.Status,
                        SuspensionReasons = s.SuspensionReasons,
                        UnitType = s.UnitType
                    });
                }

                return subscriptionsModel;
            }
            finally
            {
                subscriptions = null;
            }
        }
    }
}