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
    using Microsoft.Store.PartnerCenter.Models;
    using Microsoft.Store.PartnerCenter.Models.Auditing;
    using Microsoft.Store.PartnerCenter.Models.Licenses;
    using Microsoft.Store.PartnerCenter.Models.ServiceRequests;
    using Microsoft.Store.PartnerCenter.Models.Users;
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
        /// Checks if the specified domain is available.
        /// </summary>
        /// <param name="domain">Domain to be cheked for availability.</param>
        /// <returns><c>true</c> if the domain is available; otherwise <c>false</c></returns>
        Task<bool> CheckDomainAsync(string domain);

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
        /// <returns>An instance of <see cref="Order"/> that represents the newly created order.</returns>
        Task<Order> CreateOrderAsync(string customerId, Order newOrder);

        /// <summary>
        /// Creates the specified user.
        /// </summary>
        /// <param name="customerId">Identifier of the customer.</param>
        /// <param name="newEntity">An aptly populated instance of <see cref="CustomerUser"/>.</param>
        /// <returns>An instance of <see cref="CustomerUser"/> representing the new user.</returns>
        Task<CustomerUser> CreateUserAsync(string customerId, CustomerUser newEntity);

        /// <summary>
        /// Deletes the specified customer. This operation is only valid when connected to the 
        /// integration sandbox tenant.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task DeleteCustomerAsync(string customerId);

        /// <summary>
        /// Deletes the specified user.
        /// </summary>
        /// <param name="customerId">Identifier of the customer.</param>
        /// <param name="userId">Identifier of the user.</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task DeleteUserAsync(string customerId, string userId);

        /// <summary>
        /// Gets the specified customer.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <returns>An instance of <see cref="Customer"/> that represents the specified customer.</returns>
        Task<Customer> GetCustomerAsync(string customerId);

        /// <summary>
        /// Gets the subscribed SKUs for the specified customer.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <returns>A collection of SKUs that where the customer has subscribed.</returns>
        Task<ResourceCollection<SubscribedSku>> GetCustomerSubscribedSkusAsync(string customerId);

        /// <summary>
        /// Get the audit records available for the defined time period.
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        Task<SeekBasedResourceCollection<AuditRecord>> GetAuditRecordsAsync(DateTime startTime, DateTime endTime);

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
        /// <returns>A list of <see cref="AzureUtilizationRecord"/>s that represents usage that occurred during the specified time period.</returns>
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
        /// Gets a list of service requests.
        /// </summary>
        /// <returns>A list of service requests.</returns>
        Task<ResourceCollection<ServiceRequest>> GetServiceRequestsAsync();

        /// <summary>
        /// Gets the subscriptions for the specified customer.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <returns>A list of subscriptions for the customer.</returns>
        Task<List<Subscription>> GetSubscriptionsAsync(string customerId);

        /// <summary>
        /// Gets the specified user.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <param name="userId">Identifier for the user.</param>
        /// <returns>An instance of <see cref="CustomerUser"/> that represents the requested user.</returns>
        Task<CustomerUser> GetUserAsync(string customerId, string userId);

        /// <summary>
        /// Gets the licenses assigned to the specified user.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <param name="userId">Identifier for the user.</param>
        /// <returns>A collection of licenses assigned to the user.</returns>
        Task<ResourceCollection<License>> GetUserLicensesAsync(string customerId, string userId);

        /// <summary>
        /// Updates the specified subscription.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <param name="subscription">An instance of <see cref="Subscription"/>.</param>
        /// <returns>An instance of <see cref="Subscription"/> that represents the modified subscription.</returns>
        Task<Subscription> UpdateSubscriptionAsync(string customerId, Subscription subscription);

        /// <summary>
        /// Updates the specified user.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <param name="userId">Identifier for the user.</param>
        /// <param name="user">An aptly populated instance of the <see cref="CustomerUser"/> class.</param>
        /// <returns>An instance of <see cref="CustomerUser"/> that represents the updated user.</returns>
        Task<CustomerUser> UpdateUserAsync(string customerId, string userId, CustomerUser user);

        /// <summary>
        /// Updates the license assignments for the specified user.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <param name="userId">Identifier for the user.</param>
        /// <param name="entity">An instance of <see cref="LicenseUpdate"/> that represents the license changes to be made.</param>
        /// <returns>An instance of the <see cref="Task"/> class that represents the asynchronous operation.</returns>
        Task UpdateUserLicensesAsync(string customerId, string userId, LicenseUpdate entity);
    }
}