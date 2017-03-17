// -----------------------------------------------------------------------
// <copyright file="PartnerOperations.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web;
    using Cache;
    using PartnerCenter.Enumerators;
    using PartnerCenter.Models;
    using PartnerCenter.Models.Customers;
    using PartnerCenter.Models.Invoices;
    using PartnerCenter.Models.Offers;
    using PartnerCenter.Models.Orders;
    using PartnerCenter.Models.Subscriptions;
    using PartnerCenter.Models.Utilizations;
    using RequestContext;
    using Security;

    /// <summary>
    /// Provides the ability to perform various partner operations.
    /// </summary>
    public class PartnerOperations : IPartnerOperations
    {
        /// <summary>
        /// Key to utilized when interacting with the cache for available offers.
        /// </summary>
        private const string OffersKey = "AvailableOffers";

        /// <summary>
        /// Provides the ability to perform partner operation using app only authentication.
        /// </summary>
        private IAggregatePartner appOperations;

        /// <summary>
        /// Provides the ability to perform partner operation using app plus user authentication.
        /// </summary>
        private IAggregatePartner userOperations;

        /// <summary>
        /// Provides access to core services.
        /// </summary>
        private IExplorerService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerOperations"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="service"/> is null.
        /// </exception>
        public PartnerOperations(IExplorerService service)
        {
            service.AssertNotNull(nameof(service));
            this.service = service;
        }

        /// <summary>
        /// Creates the customer represented by the instance of <see cref="Customer"/>.
        /// </summary>
        /// <param name="customer">An instance of <see cref="Customer"/> that represents the customer to be created.</param>
        /// <returns>An instance of <see cref="Customer"/> that represents the created customer.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="customer"/> is null.
        /// </exception>
        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            Customer newEntity;
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            IPartner operations;

            customer.AssertNotNull(nameof(customer));

            try
            {
                startTime = DateTime.Now;
                correlationId = Guid.NewGuid();
                operations = await this.GetAppOperationsAsync(correlationId);

                principal = (CustomerPrincipal)HttpContext.Current.User;

                if (!principal.CustomerId.Equals(this.service.Configuration.PartnerCenterApplicationTenantId))
                {
                    throw new UnauthorizedAccessException("You are not authorized to perform this operation.");
                }

                newEntity = await operations.Customers.CreateAsync(customer);

                // Track the event measurements for analysis.
                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds }
                };

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                this.service.Telemetry.TrackEvent("CreateCustomerAsync", eventProperties, eventMetrics);

                return newEntity;
            }
            finally
            {
                eventMetrics = null;
                eventProperties = null;
                operations = null;
                principal = null;
            }
        }

        /// <summary>
        /// Creates the new order for the specified customer.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <param name="newOrder">An instance of <see cref="Order"/> that represents the new order.</param>
        /// <returns>An instance of <see cref="Order"/> that represents the newly created order.</returns>
        public async Task<Order> CreateOrderAsync(string customerId, Order newOrder)
        {
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            IPartner operations;
            Order newEntity;

            customerId.AssertNotEmpty(nameof(customerId));
            newOrder.AssertNotNull(nameof(newOrder));

            try
            {
                startTime = DateTime.Now;
                correlationId = Guid.NewGuid();
                operations = await this.GetAppOperationsAsync(correlationId);

                principal = (CustomerPrincipal)HttpContext.Current.User;

                if (principal.CustomerId.Equals(this.service.Configuration.PartnerCenterApplicationTenantId))
                {
                    newEntity = await operations.Customers.ById(customerId).Orders.CreateAsync(newOrder);
                }
                else
                {
                    newEntity = await operations.Customers.ById(principal.CustomerId).Orders.CreateAsync(newOrder);
                }

                // Track the event measurements for analysis.
                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds }
                };

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                this.service.Telemetry.TrackEvent("CreateOrderAsync", eventProperties, eventMetrics);

                return newEntity;
            }
            finally
            {
                eventMetrics = null;
                eventProperties = null;
                operations = null;
                principal = null;
            }
        }

        /// <summary>
        /// Deletes the specified customer. This operation is only valid when connected to the 
        /// integration sandbox tenant.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// </exception>
        public async Task DeleteCustomerAsync(string customerId)
        {
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            IPartner operations;

            customerId.AssertNotEmpty(nameof(customerId));

            try
            {
                startTime = DateTime.Now;
                correlationId = Guid.NewGuid();
                operations = await this.GetAppOperationsAsync(correlationId);

                principal = (CustomerPrincipal)HttpContext.Current.User;

                if (!principal.CustomerId.Equals(this.service.Configuration.PartnerCenterApplicationTenantId))
                {
                    throw new UnauthorizedAccessException("You are not authorized to perform this operation.");
                }

                await operations.Customers.ById(customerId).DeleteAsync();

                // Track the event measurements for analysis.
                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds }
                };

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                this.service.Telemetry.TrackEvent("DeleteCustomerAsync", eventProperties, eventMetrics);
            }
            finally
            {
                eventMetrics = null;
                eventProperties = null;
                operations = null;
                principal = null;
            }
        }

        /// <summary>
        /// Gets the specified customer.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <returns>An instance of <see cref="Customer"/> that represents the specified customer.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// </exception>
        public async Task<Customer> GetCustomerAsync(string customerId)
        {
            Customer customer;
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            IPartner operations;

            try
            {
                startTime = DateTime.Now;
                correlationId = Guid.NewGuid();
                operations = await this.GetAppOperationsAsync(correlationId);

                principal = (CustomerPrincipal)HttpContext.Current.User;

                if (principal.CustomerId.Equals(this.service.Configuration.PartnerCenterApplicationTenantId))
                {
                    customer = await operations.Customers.ById(customerId).GetAsync();
                }
                else
                {
                    customer = await operations.Customers.ById(principal.CustomerId).GetAsync();
                }

                // Track the event measurements for analysis.
                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds }
                };

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                this.service.Telemetry.TrackEvent("GetCustomerAsync", eventProperties, eventMetrics);

                return customer;
            }
            finally
            {
                eventMetrics = null;
                eventProperties = null;
                operations = null;
                principal = null;
            }
        }

        /// <summary>
        /// Gets the available customers.
        /// </summary>
        /// <returns>A list of available customers.</returns>
        public async Task<List<Customer>> GetCustomersAsync()
        {
            Customer customer;
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            IPartner operations;
            IResourceCollectionEnumerator<SeekBasedResourceCollection<Customer>> customersEnumerator;
            List<Customer> customers;
            SeekBasedResourceCollection<Customer> seekCustomers;

            try
            {
                startTime = DateTime.Now;
                correlationId = Guid.NewGuid();
                operations = await this.GetAppOperationsAsync(correlationId);

                principal = (CustomerPrincipal)HttpContext.Current.User;

                customers = new List<Customer>();

                if (principal.CustomerId.Equals(this.service.Configuration.PartnerCenterApplicationTenantId))
                {
                    seekCustomers = await operations.Customers.GetAsync();
                    customersEnumerator = operations.Enumerators.Customers.Create(seekCustomers);

                    while (customersEnumerator.HasValue)
                    {
                        customers.AddRange(customersEnumerator.Current.Items);
                        await customersEnumerator.NextAsync();
                    }
                }
                else
                {
                    customer = await operations.Customers.ById(principal.CustomerId).GetAsync();
                    customers.Add(customer);
                }

                // Track the event measurements for analysis.
                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds },
                    { "NumberOfCustomers", customers.Count }
                };

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                this.service.Telemetry.TrackEvent("GetCustomersAsync", eventProperties, eventMetrics);

                return customers;
            }
            finally
            {
                customersEnumerator = null;
                eventMetrics = null;
                eventProperties = null;
                operations = null;
                principal = null;
                seekCustomers = null;
            }
        }

        /// <summary>
        /// Gets the specified invoice.
        /// </summary>
        /// <param name="invoiceId">Identifier for the invoice.</param>
        /// <returns>An instance of <see cref="Invoice"/> that represents the invoice.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="invoiceId"/> is empty or null.
        /// </exception>
        public async Task<Invoice> GetInvoiceAsync(string invoiceId)
        {
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            Invoice invoice;
            IPartner operations;

            invoiceId.AssertNotEmpty(nameof(invoiceId));

            try
            {
                startTime = DateTime.Now;
                correlationId = Guid.NewGuid();
                operations = await this.GetUserOperationsAsync(correlationId);

                principal = (CustomerPrincipal)HttpContext.Current.User;

                if (!principal.CustomerId.Equals(this.service.Configuration.PartnerCenterApplicationTenantId))
                {
                    throw new UnauthorizedAccessException("You are not authorized to perform this operation.");
                }

                invoice = await operations.Invoices.ById(invoiceId).GetAsync();

                // Track the event measurements for analysis.
                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds }
                };

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                this.service.Telemetry.TrackEvent("GetInvoiceAsync", eventProperties, eventMetrics);

                return invoice;
            }
            finally
            {
                eventMetrics = null;
                eventProperties = null;
                operations = null;
                principal = null;
            }
        }

        /// <summary>
        /// Gets the line items for the specified invoice.
        /// </summary>
        /// <param name="invoiceId">Identifier for the invoice.</param>
        /// <param name="billingProvider">The provider of billing information.</param>
        /// <param name="invoiceLineItemType">Type of invoice line items.</param>
        /// <returns>A list of line items for the specified invoice.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="invoiceId"/> is empty or null.
        /// </exception>
        public async Task<List<InvoiceLineItem>> GetInvoiceLineItemsAsync(string invoiceId, BillingProvider billingProvider, InvoiceLineItemType invoiceLineItemType)
        {
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            IPartner operations;
            ResourceCollection<InvoiceLineItem> invoiceLineItems;

            invoiceId.AssertNotEmpty(nameof(invoiceId));

            try
            {
                startTime = DateTime.Now;
                correlationId = Guid.NewGuid();
                operations = await this.GetUserOperationsAsync(correlationId);

                principal = (CustomerPrincipal)HttpContext.Current.User;

                if (!principal.CustomerId.Equals(this.service.Configuration.PartnerCenterApplicationTenantId))
                {
                    throw new UnauthorizedAccessException("You are not authorized to perform this operation.");
                }

                invoiceLineItems = await operations.Invoices.ById(invoiceId).By(billingProvider, invoiceLineItemType).GetAsync();

                // Track the event measurements for analysis.
                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds },
                    { "NumberOfInvoiceLineItems", invoiceLineItems.TotalCount }
                };

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                this.service.Telemetry.TrackEvent("GetInvoiceLineItemsAsync", eventProperties, eventMetrics);

                return new List<InvoiceLineItem>(invoiceLineItems.Items);
            }
            finally
            {
                eventMetrics = null;
                eventProperties = null;
                invoiceLineItems = null;
                operations = null;
                principal = null;
            }
        }

        /// <summary>
        /// Gets all of the invoices.
        /// </summary>
        /// <returns>A list of the available invoices.</returns>
        public async Task<List<Invoice>> GetInvoicesAsync()
        {
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            IPartner operations;
            ResourceCollection<Invoice> invoices;

            try
            {
                startTime = DateTime.Now;
                correlationId = Guid.NewGuid();
                operations = await this.GetAppOperationsAsync(correlationId);

                principal = (CustomerPrincipal)HttpContext.Current.User;

                if (!principal.CustomerId.Equals(this.service.Configuration.PartnerCenterApplicationTenantId))
                {
                    throw new UnauthorizedAccessException("You are not authorized to perform this operation.");
                }

                invoices = await operations.Invoices.GetAsync();

                // Track the event measurements for analysis.
                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds },
                    { "NumberOfInvoices", invoices.TotalCount }
                };

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                this.service.Telemetry.TrackEvent("GetInvoicesAsync", eventProperties, eventMetrics);

                return new List<Invoice>(invoices.Items);
            }
            finally
            {
                eventMetrics = null;
                eventProperties = null;
                invoices = null;
                operations = null;
                principal = null;
            }
        }

        /// <summary>
        /// Gets the available offers for the configured region.
        /// </summary>
        /// <returns>A list of available offers.</returns>
        public async Task<List<Offer>> GetOffersAsync()
        {
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            IPartner operations;
            ResourceCollection<Offer> offers;

            try
            {
                startTime = DateTime.Now;
                correlationId = Guid.NewGuid();
                operations = await this.GetAppOperationsAsync(correlationId);

                principal = (CustomerPrincipal)HttpContext.Current.User;

                offers = await this.service.Cache.FetchAsync<ResourceCollection<Offer>>(CacheDatabaseType.DataStructures, OffersKey);

                if (offers == null)
                {
                    offers = await operations.Offers.ByCountry("US").GetAsync();
                    await this.service.Cache.StoreAsync(CacheDatabaseType.DataStructures, OffersKey, offers, TimeSpan.FromDays(1));
                }

                // Track the event measurements for analysis.
                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds },
                    { "NumberOfInvoices", offers.TotalCount }
                };

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                this.service.Telemetry.TrackEvent("GetOffersAsync", eventProperties, eventMetrics);

                return new List<Offer>(offers.Items);
            }
            finally
            {
                eventMetrics = null;
                eventProperties = null;
                offers = null;
                operations = null;
                principal = null;
            }
        }

        /// <summary>
        /// Gets the specified subscription.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <param name="subscriptionId">Identifier for the subscription.</param>
        /// <returns>An instance of <see cref="Subscription"/> that represents the subscription.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// or 
        /// <paramref name="subscriptionId"/> is empty or null.
        /// </exception>
        public async Task<Subscription> GetSubscriptionAsync(string customerId, string subscriptionId)
        {
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            IPartner operations;
            Subscription subscription;

            customerId.AssertNotEmpty(nameof(customerId));
            subscriptionId.AssertNotEmpty(nameof(subscriptionId));

            try
            {
                startTime = DateTime.Now;
                correlationId = Guid.NewGuid();
                operations = await this.GetAppOperationsAsync(correlationId);

                principal = (CustomerPrincipal)HttpContext.Current.User;

                if (principal.CustomerId.Equals(this.service.Configuration.PartnerCenterApplicationTenantId))
                {
                    subscription = await operations.Customers.ById(customerId).Subscriptions.ById(subscriptionId).GetAsync();
                }
                else
                {
                    subscription = await operations.Customers.ById(principal.CustomerId).Subscriptions.ById(subscriptionId).GetAsync();
                }

                // Track the event measurements for analysis.
                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds }
                };

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                this.service.Telemetry.TrackEvent("GetSubscriptionAsync", eventProperties, eventMetrics);

                return subscription;
            }
            finally
            {
                eventMetrics = null;
                eventProperties = null;
                operations = null;
                principal = null;
            }
        }

        /// <summary>
        /// Gets the subscriptions for the specified customer.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <returns>A list of subscriptions for the customer.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// </exception>
        public async Task<List<Subscription>> GetSubscriptionsAsync(string customerId)
        {
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            IPartner operations;
            ResourceCollection<Subscription> subscriptions;

            customerId.AssertNotEmpty(nameof(customerId));

            try
            {
                startTime = DateTime.Now;
                correlationId = Guid.NewGuid();
                operations = await this.GetAppOperationsAsync(correlationId);

                principal = (CustomerPrincipal)HttpContext.Current.User;

                if (principal.CustomerId.Equals(this.service.Configuration.PartnerCenterApplicationTenantId))
                {
                    subscriptions = await operations.Customers.ById(customerId).Subscriptions.GetAsync();
                }
                else
                {
                    subscriptions = await operations.Customers.ById(principal.CustomerId).Subscriptions.GetAsync();
                }

                // Track the event measurements for analysis.
                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds },
                    { "NumberOfSubscriptions", subscriptions.TotalCount }
                };

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                this.service.Telemetry.TrackEvent("GetSubscriptionsAsync", eventProperties, eventMetrics);

                return new List<Subscription>(subscriptions.Items);
            }
            finally
            {
                eventMetrics = null;
                eventProperties = null;
                operations = null;
                principal = null;
            }
        }

        /// <summary>
        /// Gets the usage for the specified subscription.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <param name="subscriptionId">Identifier for the subscription.</param>
        /// <param name="startTime">The starting time of when the utilization was metered in the billing system.</param>
        /// <param name="endTime">The ending time of when the utilization was metered in the billing system.</param>
        /// <returns>A list of <see cref="AzureUtilizationRecord"/>s that represents usage that occurred during the specified time period.</returns>
        public async Task<List<AzureUtilizationRecord>> GetSubscriptionUsageAsync(string customerId, string subscriptionId, DateTime startTime, DateTime endTime)
        {
            CustomerPrincipal principal;
            DateTime invokeTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            IPartner operations;
            IResourceCollectionEnumerator<ResourceCollection<AzureUtilizationRecord>> usageEnumerator;
            List<AzureUtilizationRecord> usageRecords;
            ResourceCollection<AzureUtilizationRecord> records;

            customerId.AssertNotEmpty(nameof(customerId));
            subscriptionId.AssertNotEmpty(nameof(subscriptionId));

            try
            {
                invokeTime = DateTime.Now;
                correlationId = Guid.NewGuid();
                operations = await this.GetAppOperationsAsync(correlationId);

                principal = (CustomerPrincipal)HttpContext.Current.User;

                usageRecords = new List<AzureUtilizationRecord>();

                if (principal.CustomerId.Equals(this.service.Configuration.PartnerCenterApplicationTenantId))
                {
                    records = await operations.Customers.ById(customerId).Subscriptions.ById(subscriptionId)
                        .Utilization.Azure.QueryAsync(startTime, endTime);
                    usageEnumerator = operations.Enumerators.Utilization.Azure.Create(records);

                    while (usageEnumerator.HasValue)
                    {
                        usageRecords.AddRange(usageEnumerator.Current.Items);
                        await usageEnumerator.NextAsync();
                    }
                }
                else
                {
                    records = await operations.Customers.ById(principal.CustomerId).Subscriptions.ById(subscriptionId)
                        .Utilization.Azure.QueryAsync(startTime, endTime);
                    usageEnumerator = operations.Enumerators.Utilization.Azure.Create(records);

                    while (usageEnumerator.HasValue)
                    {
                        usageRecords.AddRange(usageEnumerator.Current.Items);
                        await usageEnumerator.NextAsync();
                    }
                }

                // Track the event measurements for analysis.
                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(invokeTime).TotalMilliseconds },
                    { "NumberOfUsageRecords", usageRecords.Count }
                };

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                this.service.Telemetry.TrackEvent("GetSubscriptionUsageAsync", eventProperties, eventMetrics);

                return usageRecords;
            }
            finally
            {
                eventMetrics = null;
                eventProperties = null;
                operations = null;
                principal = null;
                records = null;
                usageEnumerator = null;
            }
        }

        /// <summary>
        /// Updates the specified subscription.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <param name="subscription">An instance of <see cref="Subscription"/>.</param>
        /// <returns>An instance of <see cref="Subscription"/> that represents the modified subscription.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="subscription"/> is empty or null.
        /// </exception>
        public async Task<Subscription> UpdateSubscriptionAsync(string customerId, Subscription subscription)
        {
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            IPartner operations;
            Subscription updatedSubscription;

            customerId.AssertNotEmpty(nameof(customerId));
            subscription.AssertNotNull(nameof(subscription));

            try
            {
                startTime = DateTime.Now;
                correlationId = Guid.NewGuid();
                operations = await this.GetAppOperationsAsync(correlationId);

                principal = (CustomerPrincipal)HttpContext.Current.User;

                if (principal.CustomerId.Equals(this.service.Configuration.PartnerCenterApplicationTenantId))
                {
                    updatedSubscription = await operations.Customers.ById(customerId).Subscriptions
                        .ById(subscription.Id).PatchAsync(subscription);
                }
                else
                {
                    updatedSubscription = await operations.Customers.ById(principal.CustomerId).Subscriptions
                        .ById(subscription.Id).PatchAsync(subscription);
                }

                // Track the event measurements for analysis.
                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds }
                };

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                this.service.Telemetry.TrackEvent("GetSubscriptionAsync", eventProperties, eventMetrics);

                return updatedSubscription;
            }
            finally
            {
                eventMetrics = null;
                eventProperties = null;
                operations = null;
                principal = null;
            }
        }

        /// <summary>
        /// Gets an instance of the partner service that utilizes app only authentication.
        /// </summary>
        /// <param name="correlationId">Correlation identifier for the operation.</param>
        /// <returns>An instance of the partner service.</returns>
        private async ValueTask<IPartner> GetAppOperationsAsync(Guid correlationId)
        {
            if (this.appOperations == null || this.appOperations.Credentials.ExpiresAt > DateTime.UtcNow)
            {
                IPartnerCredentials credentials = await this.service.TokenManagement
                    .GetPartnerCenterAppOnlyCredentialsAsync(
                        $"{this.service.Configuration.ActiveDirectoryEndpoint}/{this.service.Configuration.PartnerCenterApplicationTenantId}");

                this.appOperations = PartnerService.Instance.CreatePartnerOperations(credentials);
            }

            return this.appOperations.With(RequestContextFactory.Instance.Create(correlationId));
        }

        /// <summary>
        /// Gets an instance of the partner service that utilizes app plus user authentication.
        /// </summary>
        /// <param name="correlationId">Correlation identifier for the operation.</param>
        /// <returns>An instance of the partner service.</returns>
        private async ValueTask<IPartner> GetUserOperationsAsync(Guid correlationId)
        {
            if (this.userOperations == null || this.userOperations.Credentials.ExpiresAt > DateTime.UtcNow)
            {
                IPartnerCredentials credentials = await this.service.TokenManagement
                    .GetPartnerCenterAppPlusUserCredentialsAsync(
                        $"{this.service.Configuration.ActiveDirectoryEndpoint}/{this.service.Configuration.PartnerCenterApplicationTenantId}");

                this.userOperations = PartnerService.Instance.CreatePartnerOperations(credentials);
            }

            return this.userOperations.With(RequestContextFactory.Instance.Create(correlationId));
        }
    }
}