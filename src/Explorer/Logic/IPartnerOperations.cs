// -----------------------------------------------------------------------
// <copyright file="IPartnerOperations.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PartnerCenter.Models.Customers;
    using PartnerCenter.Models.Invoices;
    using PartnerCenter.Models.Offers;
    using PartnerCenter.Models.Orders;
    using PartnerCenter.Models.Subscriptions;
    using PartnerCenter.Models.Utilizations;

    /// <summary>
    /// Represents the ability to perform various partner operations.
    /// </summary>
    public interface IPartnerOperations
    {
        /// <summary>
        /// Creates the customer represented by the instance of <see cref="Customer"/>.
        /// </summary>
        /// <param name="customer">An instance of <see cref="Customer"/> that represents the customer to be created.</param>
        /// <returns>An instance of <see cref="Customer"/> that represents the created customer.</returns>
        Task<Customer> CreateCustomerAsync(Customer customer);

        /// <summary>
        /// Creates the new order for the specified customer.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <param name="newOrder">An instance of <see cref="Order"/> that represents the new order.</param>
        /// <returns>An instance of <see cref="Order"/> that represents the new order.</returns>
        Task<Order> CreateOrderAsync(string customerId, Order newOrder);

        /// <summary>
        /// Deletes the specified custmer. This operation is only valid when connected to the 
        /// integration sandbox tenant.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task DeleteCustomerAsync(string customerId);

        /// <summary>
        /// Gets the specified customer.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <returns>An instance of <see cref="Customer"/> that represents the specified customer.</returns>
        Task<Customer> GetCustomerAsync(string customerId);

        /// <summary>
        /// Gets the available customers.
        /// </summary>
        /// <returns>A list of available customers.</returns>
        Task<List<Customer>> GetCustomersAsync();

        /// <summary>
        /// Gets the specified subscription.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <param name="subscriptionId">Identifier for the subscription.</param>
        /// <returns>An instance of <see cref="Subscription"/> that represents the subscription.</returns>
        Task<Subscription> GetSubscriptionAsync(string customerId, string subscriptionId);

        /// <summary>
        /// Gets the usage for the specified subscription.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <param name="subscriptionId">Identifier for the subscription.</param>
        /// <param name="startTime">The starting time of when the utilization was metered in the billing system.</param>
        /// <param name="endTime">The ending time of when the utilization was metered in the billing system.</param>
        /// <returns>A list of <see cref="AzureUtilizationRecord"/>s that represents usage that occured during the specified time period.</returns>
        Task<List<AzureUtilizationRecord>> GetSubscriptionUsageAsync(string customerId, string subscriptionId, DateTime startTime, DateTime endTime);

        /// <summary>
        /// Gets the specified invoice.
        /// </summary>
        /// <param name="invoiceId">Identifier for the invoice.</param>
        /// <returns>An instance of <see cref="Invoice"/> that represents the invoice.</returns>
        Task<Invoice> GetInvoiceAsync(string invoiceId);

        /// <summary>
        /// Gets the line items for the specified invoice.
        /// </summary>
        /// <param name="invoiceId">Identifier for the invoice.</param>
        /// <param name="billingProvider">The provider of billing information.</param>
        /// <param name="invoiceLineItemType">Type of invoice line items.</param>
        /// <returns>A list of line items for the specified invoice.</returns>
        Task<List<InvoiceLineItem>> GetInvoiceLineItemsAsync(string invoiceId, BillingProvider billingProvider, InvoiceLineItemType invoiceLineItemType);

        /// <summary>
        /// Gets all of the invoices.
        /// </summary>
        /// <returns>A list of the available invoices.</returns>
        Task<List<Invoice>> GetInvoicesAsync();

        /// <summary>
        /// Gets the available offers for the configured region.
        /// </summary>
        /// <returns>A list of available offers.</returns>
        Task<List<Offer>> GetOffersAsync();

        /// <summary>
        /// Gets the subscriptions for the specified customer.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <returns>A list of subscriptions for the customer.</returns>
        Task<List<Subscription>> GetSubscriptionsAsync(string customerId);

        /// <summary>
        /// Updates the specified subscription.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <param name="subscription">An instance of <see cref="Subscription"/> that represents the modified subscription.</param>
        /// <returns>An instance of <see cref="Subscription"/> that represents the modified subscription.</returns>
        Task<Subscription> UpdateSubscriptionAsync(string customerId, Subscription subscription);
    }
}