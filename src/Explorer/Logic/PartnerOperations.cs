// -----------------------------------------------------------------------
// <copyright file="PartnerOperations.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Cache;
    using IdentityModel.Clients.ActiveDirectory;
    using PartnerCenter.Enumerators;
    using PartnerCenter.Extensions;
    using PartnerCenter.Models;
    using PartnerCenter.Models.Auditing;
    using PartnerCenter.Models.Customers;
    using PartnerCenter.Models.Invoices;
    using PartnerCenter.Models.Licenses;
    using PartnerCenter.Models.Offers;
    using PartnerCenter.Models.Orders;
    using PartnerCenter.Models.Query;
    using PartnerCenter.Models.ServiceRequests;
    using PartnerCenter.Models.Subscriptions;
    using PartnerCenter.Models.Users;
    using PartnerCenter.Models.Utilizations;
    using Providers;
    using RequestContext;
    using Security;

    /// <summary>
    /// Provides the ability to perform various partner operations.
    /// </summary>
    public class PartnerOperations : IPartnerOperations
    {
        /// <summary>
        /// Name of the application calling the Partner Center Managed API.
        /// </summary>
        private const string ApplicationName = "Partner Center Explorer v2.0";

        /// <summary>
        /// Key to utilized when interacting with the cache for available offers.
        /// </summary>
        private const string OffersKey = "AvailableOffers";

        /// <summary>
        /// Key utilized to retrieve and store Partner Center access tokens. 
        /// </summary>
        private const string PartnerCenterCacheKey = "Resource::PartnerCenter::AppOnly";

        /// <summary>
        /// Provides the ability to perform partner operation using app only authentication.
        /// </summary>
        private IAggregatePartner appOperations;

        /// <summary>
        /// Provides access to core services.
        /// </summary>
        private IExplorerProvider provider;

        /// <summary>
        /// Provides a way to ensure that <see cref="appOperations"/> is only being modified 
        /// by one thread at a time. 
        /// </summary>
        private readonly object appLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerOperations"/> class.
        /// </summary>
        /// <param name="provider">Provides access to core services.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="provider"/> is null.
        /// </exception>
        public PartnerOperations(IExplorerProvider provider)
        {
            provider.AssertNotNull(nameof(provider));
            this.provider = provider;
        }

        /// <summary>
        /// Checks if the specified domain is available.
        /// </summary>
        /// <param name="domain">Domain to be cheked for availability.</param>
        /// <returns><c>true</c> if the domain is available; otherwise <c>false</c></returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="domain"/> is empty or null.
        /// </exception>
        public async Task<bool> CheckDomainAsync(string domain)
        {
            CustomerPrincipal principal;
            DateTime executionTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            IPartner operations;
            bool exists;

            domain.AssertNotEmpty(nameof(domain));

            try
            {
                executionTime = DateTime.Now;
                correlationId = Guid.NewGuid();
                operations = await GetAppOperationsAsync(correlationId).ConfigureAwait(false);

                principal = new CustomerPrincipal(ClaimsPrincipal.Current);

                exists = await operations.Domains.ByDomain(domain).ExistsAsync().ConfigureAwait(false);

                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(executionTime).TotalMilliseconds }
                };

                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Domain", domain },
                    { "Exists", exists.ToString() },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                provider.Telemetry.TrackEvent(nameof(CheckDomainAsync), eventProperties, eventMetrics);

                return exists;
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
                operations = await GetAppOperationsAsync(correlationId).ConfigureAwait(false);

                principal = new CustomerPrincipal(ClaimsPrincipal.Current);

                if (!principal.CustomerId.Equals(provider.Configuration.PartnerCenterApplicationTenantId))
                {
                    throw new UnauthorizedAccessException("You are not authorized to perform this operation.");
                }

                newEntity = await operations.Customers.CreateAsync(customer).ConfigureAwait(false);

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

                provider.Telemetry.TrackEvent(nameof(CreateCustomerAsync), eventProperties, eventMetrics);

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
                operations = await GetAppOperationsAsync(correlationId).ConfigureAwait(false);

                principal = new CustomerPrincipal(ClaimsPrincipal.Current);

                if (principal.CustomerId.Equals(provider.Configuration.PartnerCenterApplicationTenantId))
                {
                    newEntity = await operations.Customers.ById(customerId).Orders.CreateAsync(newOrder);
                }
                else
                {
                    newEntity = await operations.Customers.ById(principal.CustomerId).Orders.CreateAsync(newOrder).ConfigureAwait(false);
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

                provider.Telemetry.TrackEvent(nameof(CreateOrderAsync), eventProperties, eventMetrics);

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
        /// Creates the specified user.
        /// </summary>
        /// <param name="customerId">Identifier of the customer.</param>
        /// <param name="newEntity">An aptly populated instance of <see cref="CustomerUser"/>.</param>
        /// <returns>An instance of <see cref="CustomerUser"/> representing the new user.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="newEntity"/> is null.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// The authenticated user is not authorized to perform this operation.
        /// </exception>
        public async Task<CustomerUser> CreateUserAsync(string customerId, CustomerUser newEntity)
        {
            CustomerPrincipal principal;
            CustomerUser user;
            DateTime executionTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            IPartner operations;

            customerId.AssertNotEmpty(nameof(customerId));
            newEntity.AssertNotNull(nameof(user));

            try
            {
                executionTime = DateTime.Now;
                correlationId = Guid.NewGuid();
                operations = await GetAppOperationsAsync(correlationId).ConfigureAwait(false);

                principal = new CustomerPrincipal(ClaimsPrincipal.Current);

                if (principal.CustomerId.Equals(provider.Configuration.PartnerCenterApplicationTenantId))
                {
                    user = await operations.Customers.ById(customerId).Users.CreateAsync(newEntity).ConfigureAwait(false);
                }
                else
                {
                    throw new UnauthorizedAccessException("You are not authorized to perform this operation.");
                }

                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(executionTime).TotalMilliseconds }
                };

                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                provider.Telemetry.TrackEvent(nameof(CreateUserAsync), eventProperties, eventMetrics);

                return user;
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
                operations = await GetAppOperationsAsync(correlationId).ConfigureAwait(false);

                principal = new CustomerPrincipal(ClaimsPrincipal.Current);

                if (!principal.CustomerId.Equals(provider.Configuration.PartnerCenterApplicationTenantId))
                {
                    throw new UnauthorizedAccessException("You are not authorized to perform this operation.");
                }

                await operations.Customers.ById(customerId).DeleteAsync().ConfigureAwait(false);

                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds }
                };

                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                provider.Telemetry.TrackEvent(nameof(DeleteCustomerAsync), eventProperties, eventMetrics);
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
        /// Deletes the specified user.
        /// </summary>
        /// <param name="customerId">Identifier of the customer.</param>
        /// <param name="userId">Identifier of the user.</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// or
        /// <paramref name="userId"/> is empty or null.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// The authenticated user is not authorized to perform this operation.
        /// </exception>
        public async Task DeleteUserAsync(string customerId, string userId)
        {
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            IPartner operations;

            customerId.AssertNotEmpty(nameof(customerId));
            userId.AssertNotEmpty(nameof(userId));

            try
            {
                startTime = DateTime.Now;
                correlationId = Guid.NewGuid();
                operations = await GetAppOperationsAsync(correlationId).ConfigureAwait(false);

                principal = new CustomerPrincipal(ClaimsPrincipal.Current);

                if (!principal.CustomerId.Equals(provider.Configuration.PartnerCenterApplicationTenantId))
                {
                    throw new UnauthorizedAccessException("You are not authorized to perform this operation.");
                }

                await operations.Customers.ById(customerId).Users.ById(userId).DeleteAsync().ConfigureAwait(false);

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

                provider.Telemetry.TrackEvent(nameof(DeleteUserAsync), eventProperties, eventMetrics);
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
        /// Get the audit records available for the defined time period.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<SeekBasedResourceCollection<AuditRecord>> GetAuditRecordsAsync(DateTime startDate, DateTime endDate)
        {
            CustomerPrincipal principal;
            DateTime executionTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            IPartner operations;
            SeekBasedResourceCollection<AuditRecord> records;

            try
            {
                executionTime = DateTime.Now;
                correlationId = Guid.NewGuid();
                operations = await GetAppOperationsAsync(correlationId).ConfigureAwait(false);

                principal = new CustomerPrincipal(ClaimsPrincipal.Current);

                records = await operations.AuditRecords.QueryAsync(
                    startDate,
                    endDate,
                    QueryFactory.Instance.BuildSimpleQuery()).ConfigureAwait(false);

                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(executionTime).TotalMilliseconds },
                    { "NumberOfAuditRecords", records.TotalCount }
                };

                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                provider.Telemetry.TrackEvent(nameof(GetAuditRecordsAsync), eventProperties, eventMetrics);

                return records;
            }
            finally
            {

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
                operations = await GetAppOperationsAsync(correlationId).ConfigureAwait(false);

                principal = new CustomerPrincipal(ClaimsPrincipal.Current);

                if (principal.CustomerId.Equals(provider.Configuration.PartnerCenterApplicationTenantId))
                {
                    customer = await operations.Customers.ById(customerId).GetAsync().ConfigureAwait(false);
                }
                else
                {
                    customer = await operations.Customers.ById(principal.CustomerId).GetAsync().ConfigureAwait(false);
                }

                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds }
                };

                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                provider.Telemetry.TrackEvent(nameof(GetCustomerAsync), eventProperties, eventMetrics);

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
                operations = await GetAppOperationsAsync(correlationId).ConfigureAwait(false);

                principal = new CustomerPrincipal(ClaimsPrincipal.Current);

                customers = new List<Customer>();

                if (principal.CustomerId.Equals(provider.Configuration.PartnerCenterApplicationTenantId))
                {
                    seekCustomers = await operations.Customers.GetAsync().ConfigureAwait(false);
                    customersEnumerator = operations.Enumerators.Customers.Create(seekCustomers);

                    while (customersEnumerator.HasValue)
                    {
                        customers.AddRange(customersEnumerator.Current.Items);
                        await customersEnumerator.NextAsync().ConfigureAwait(false);
                    }
                }
                else
                {
                    customer = await operations.Customers.ById(principal.CustomerId).GetAsync().ConfigureAwait(false);
                    customers.Add(customer);
                }

                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds },
                    { "NumberOfCustomers", customers.Count }
                };

                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                provider.Telemetry.TrackEvent(nameof(GetCustomersAsync), eventProperties, eventMetrics);

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
        /// Gets the subscribed SKUs for the specified customer.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <returns>A collection of SKUs that where the customer has subscribed.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// </exception>
        public async Task<ResourceCollection<SubscribedSku>> GetCustomerSubscribedSkusAsync(string customerId)
        {
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            IPartner operations;
            ResourceCollection<SubscribedSku> skus;

            customerId.AssertNotEmpty(nameof(customerId));

            try
            {
                startTime = DateTime.Now;
                correlationId = Guid.NewGuid();
                operations = await GetUserOperationsAsync(correlationId).ConfigureAwait(false);

                principal = new CustomerPrincipal(ClaimsPrincipal.Current);

                if (principal.CustomerId.Equals(provider.Configuration.PartnerCenterApplicationTenantId) ||
                    principal.CustomerId.Equals(customerId))
                {
                    skus = await operations.Customers.ById(customerId).SubscribedSkus.GetAsync().ConfigureAwait(false);
                }
                else
                {
                    throw new UnauthorizedAccessException("You are not authorized to perform this operation.");
                }

                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds }
                };

                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                provider.Telemetry.TrackEvent(nameof(GetCustomerSubscribedSkusAsync), eventProperties, eventMetrics);

                return skus;
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
                operations = await GetUserOperationsAsync(correlationId).ConfigureAwait(false);

                principal = new CustomerPrincipal(ClaimsPrincipal.Current);

                if (!principal.CustomerId.Equals(provider.Configuration.PartnerCenterApplicationTenantId))
                {
                    throw new UnauthorizedAccessException("You are not authorized to perform this operation.");
                }

                invoice = await operations.Invoices.ById(invoiceId).GetAsync().ConfigureAwait(false);

                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds }
                };

                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                provider.Telemetry.TrackEvent(nameof(GetInvoiceAsync), eventProperties, eventMetrics);

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
                operations = await GetUserOperationsAsync(correlationId).ConfigureAwait(false);

                principal = new CustomerPrincipal(ClaimsPrincipal.Current);

                if (!principal.CustomerId.Equals(provider.Configuration.PartnerCenterApplicationTenantId))
                {
                    throw new UnauthorizedAccessException("You are not authorized to perform this operation.");
                }

                invoiceLineItems = await operations.Invoices.ById(invoiceId).By(billingProvider, invoiceLineItemType).GetAsync().ConfigureAwait(false);

                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds },
                    { "NumberOfInvoiceLineItems", invoiceLineItems.TotalCount }
                };

                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                provider.Telemetry.TrackEvent(nameof(GetInvoiceLineItemsAsync), eventProperties, eventMetrics);

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
                operations = await GetAppOperationsAsync(correlationId).ConfigureAwait(false);

                principal = new CustomerPrincipal(ClaimsPrincipal.Current);

                if (!principal.CustomerId.Equals(provider.Configuration.PartnerCenterApplicationTenantId))
                {
                    throw new UnauthorizedAccessException("You are not authorized to perform this operation.");
                }

                invoices = await operations.Invoices.GetAsync().ConfigureAwait(false);

                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds },
                    { "NumberOfInvoices", invoices.TotalCount }
                };

                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                provider.Telemetry.TrackEvent("GetInvoicesAsync", eventProperties, eventMetrics);

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
                operations = await GetAppOperationsAsync(correlationId).ConfigureAwait(false);

                principal = new CustomerPrincipal(ClaimsPrincipal.Current);

                offers = await provider.Cache.FetchAsync<ResourceCollection<Offer>>(CacheDatabaseType.DataStructures, OffersKey).ConfigureAwait(false);

                if (offers == null)
                {
                    offers = await operations.Offers.ByCountry("US").GetAsync();
                    await provider.Cache.StoreAsync(CacheDatabaseType.DataStructures, OffersKey, offers, TimeSpan.FromDays(1)).ConfigureAwait(false);
                }

                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds },
                    { "NumberOfInvoices", offers.TotalCount }
                };

                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                provider.Telemetry.TrackEvent(nameof(GetOffersAsync), eventProperties, eventMetrics);

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
        /// Gets a list of service requests.
        /// </summary>
        /// <returns>A list of service requests.</returns>
        public async Task<ResourceCollection<ServiceRequest>> GetServiceRequestsAsync()
        {
            CustomerPrincipal principal;
            DateTime executionTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            IPartner operations;
            ResourceCollection<ServiceRequest> requests;

            try
            {
                executionTime = DateTime.Now;
                correlationId = Guid.NewGuid();
                operations = await GetAppOperationsAsync(correlationId).ConfigureAwait(false);

                principal = new CustomerPrincipal(ClaimsPrincipal.Current);

                if (principal.CustomerId.Equals(provider.Configuration.PartnerCenterApplicationTenantId))
                {
                    requests = await operations.ServiceRequests.GetAsync().ConfigureAwait(false);
                }
                else
                {
                    requests = new ResourceCollection<ServiceRequest>(new List<ServiceRequest>());
                }

                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(executionTime).TotalMilliseconds },
                    { "NumberOfRequests", requests.TotalCount }
                };

                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                provider.Telemetry.TrackEvent(nameof(GetServiceRequestsAsync), eventProperties, eventMetrics);

                return requests;
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
                operations = await GetAppOperationsAsync(correlationId).ConfigureAwait(false);

                principal = new CustomerPrincipal(ClaimsPrincipal.Current);

                if (principal.CustomerId.Equals(provider.Configuration.PartnerCenterApplicationTenantId))
                {
                    subscription = await operations.Customers.ById(customerId).Subscriptions.ById(subscriptionId).GetAsync().ConfigureAwait(false);
                }
                else
                {
                    subscription = await operations.Customers.ById(principal.CustomerId).Subscriptions.ById(subscriptionId).GetAsync().ConfigureAwait(false);
                }

                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds }
                };

                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                provider.Telemetry.TrackEvent(nameof(GetSubscriptionAsync), eventProperties, eventMetrics);

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
                operations = await GetAppOperationsAsync(correlationId).ConfigureAwait(false);

                principal = new CustomerPrincipal(ClaimsPrincipal.Current);

                if (principal.CustomerId.Equals(provider.Configuration.PartnerCenterApplicationTenantId))
                {
                    subscriptions = await operations.Customers.ById(customerId).Subscriptions.GetAsync().ConfigureAwait(false);
                }
                else
                {
                    subscriptions = await operations.Customers.ById(principal.CustomerId).Subscriptions.GetAsync().ConfigureAwait(false);
                }

                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds },
                    { "NumberOfSubscriptions", subscriptions.TotalCount }
                };

                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                provider.Telemetry.TrackEvent(nameof(GetSubscriptionsAsync), eventProperties, eventMetrics);

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
                operations = await GetAppOperationsAsync(correlationId).ConfigureAwait(false);

                principal = new CustomerPrincipal(ClaimsPrincipal.Current);

                usageRecords = new List<AzureUtilizationRecord>();

                if (principal.CustomerId.Equals(provider.Configuration.PartnerCenterApplicationTenantId))
                {
                    records = await operations.Customers.ById(customerId).Subscriptions.ById(subscriptionId)
                        .Utilization.Azure.QueryAsync(startTime, endTime).ConfigureAwait(false);
                    usageEnumerator = operations.Enumerators.Utilization.Azure.Create(records);

                    while (usageEnumerator.HasValue)
                    {
                        usageRecords.AddRange(usageEnumerator.Current.Items);
                        await usageEnumerator.NextAsync().ConfigureAwait(false);
                    }
                }
                else
                {
                    records = await operations.Customers.ById(principal.CustomerId).Subscriptions.ById(subscriptionId)
                        .Utilization.Azure.QueryAsync(startTime, endTime).ConfigureAwait(false);
                    usageEnumerator = operations.Enumerators.Utilization.Azure.Create(records);

                    while (usageEnumerator.HasValue)
                    {
                        usageRecords.AddRange(usageEnumerator.Current.Items);
                        await usageEnumerator.NextAsync().ConfigureAwait(false);
                    }
                }

                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(invokeTime).TotalMilliseconds },
                    { "NumberOfUsageRecords", usageRecords.Count }
                };

                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                provider.Telemetry.TrackEvent(nameof(GetSubscriptionUsageAsync), eventProperties, eventMetrics);

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
        /// Gets the specified user.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <param name="userId">Identifier for the user.</param>
        /// <returns>An instance of <see cref="CustomerUser"/> that represents the requested user.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// or
        /// <paramref name="userId"/> is empty or null.
        /// </exception>
        public async Task<CustomerUser> GetUserAsync(string customerId, string userId)
        {
            CustomerPrincipal principal;
            CustomerUser user;
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            IPartner operations;

            customerId.AssertNotEmpty(nameof(customerId));
            userId.AssertNotEmpty(nameof(userId));

            try
            {
                startTime = DateTime.Now;
                correlationId = Guid.NewGuid();
                operations = await GetUserOperationsAsync(correlationId).ConfigureAwait(false);

                principal = new CustomerPrincipal(ClaimsPrincipal.Current);

                if (principal.CustomerId.Equals(provider.Configuration.PartnerCenterApplicationTenantId) ||
                    principal.CustomerId.Equals(customerId))
                {
                    user = await operations.Customers.ById(customerId).Users.ById(userId).GetAsync().ConfigureAwait(false);
                }
                else
                {
                    throw new UnauthorizedAccessException("You are not authorized to perform this operation.");
                }

                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds }
                };

                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "UserId", userId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                provider.Telemetry.TrackEvent(nameof(GetUserAsync), eventProperties, eventMetrics);

                return user;
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
        /// Gets the licenses assigned to the specified user.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <param name="userId">Identifier for the user.</param>
        /// <returns>A collection of licenses assigned to the user.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// or
        /// <paramref name="userId"/> is empty or null.
        /// </exception>
        public async Task<ResourceCollection<License>> GetUserLicensesAsync(string customerId, string userId)
        {
            CustomerPrincipal principal;
            DateTime executionTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            IPartner operations;
            ResourceCollection<License> licenses;

            customerId.AssertNotEmpty(nameof(customerId));
            userId.AssertNotEmpty(nameof(userId));

            try
            {
                executionTime = DateTime.Now;
                correlationId = Guid.NewGuid();
                operations = await GetUserOperationsAsync(correlationId).ConfigureAwait(false);

                principal = new CustomerPrincipal(ClaimsPrincipal.Current);

                if (principal.CustomerId.Equals(provider.Configuration.PartnerCenterApplicationTenantId) ||
                    principal.CustomerId.Equals(customerId))
                {
                    licenses = await operations.Customers.ById(customerId).Users.ById(userId).Licenses.GetAsync().ConfigureAwait(false);
                }
                else
                {
                    throw new UnauthorizedAccessException("You are not authorized to perform this operation.");
                }

                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(executionTime).TotalMilliseconds }
                };

                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "UserId", userId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                provider.Telemetry.TrackEvent(nameof(GetUserLicensesAsync), eventProperties, eventMetrics);

                return licenses;
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
                operations = await GetAppOperationsAsync(correlationId).ConfigureAwait(false);

                principal = new CustomerPrincipal(ClaimsPrincipal.Current);

                if (principal.CustomerId.Equals(provider.Configuration.PartnerCenterApplicationTenantId))
                {
                    updatedSubscription = await operations.Customers.ById(customerId).Subscriptions
                        .ById(subscription.Id).PatchAsync(subscription).ConfigureAwait(false);
                }
                else
                {
                    updatedSubscription = await operations.Customers.ById(principal.CustomerId).Subscriptions
                        .ById(subscription.Id).PatchAsync(subscription).ConfigureAwait(false);
                }

                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds }
                };

                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                provider.Telemetry.TrackEvent(nameof(UpdateSubscriptionAsync), eventProperties, eventMetrics);

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
        /// Updates the specified user.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <param name="userId">Identifier for the user.</param>
        /// <param name="entity">An aptly populated instance of the <see cref="CustomerUser"/> class.</param>
        /// <returns>An instance of <see cref="CustomerUser"/> that represents the updated user.</returns>
        public async Task<CustomerUser> UpdateUserAsync(string customerId, string userId, CustomerUser entity)
        {
            CustomerPrincipal principal;
            CustomerUser user;
            DateTime executionTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            IPartner operations;

            customerId.AssertNotEmpty(nameof(customerId));
            userId.AssertNotEmpty(nameof(userId));
            entity.AssertNotNull(nameof(entity));

            try
            {
                executionTime = DateTime.Now;
                correlationId = Guid.NewGuid();
                operations = await GetUserOperationsAsync(correlationId).ConfigureAwait(false);

                principal = new CustomerPrincipal(ClaimsPrincipal.Current);

                if (principal.CustomerId.Equals(provider.Configuration.PartnerCenterApplicationTenantId) ||
                    principal.CustomerId.Equals(customerId))
                {
                    user = await operations.Customers.ById(customerId).Users.ById(userId).PatchAsync(entity).ConfigureAwait(false);
                }
                else
                {
                    throw new UnauthorizedAccessException("You are not authorized to perform this operation.");
                }

                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(executionTime).TotalMilliseconds }
                };

                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                provider.Telemetry.TrackEvent(nameof(UpdateUserAsync), eventProperties, eventMetrics);

                return user;
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
        /// Updates the license assignments for the specified user.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <param name="userId">Identifier for the user.</param>
        /// <param name="entity">An instance of <see cref="LicenseUpdate"/> that represents the license changes to be made.</param>
        /// <returns>An instance of the <see cref="Task"/> class that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// or 
        /// <paramref name="userId"/> is empty or null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entity"/> is null.
        /// </exception>
        public async Task UpdateUserLicensesAsync(string customerId, string userId, LicenseUpdate entity)
        {
            CustomerPrincipal principal;
            DateTime executionTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            IPartner operations;

            customerId.AssertNotEmpty(nameof(customerId));
            userId.AssertNotEmpty(nameof(userId));
            entity.AssertNotNull(nameof(entity));

            try
            {
                executionTime = DateTime.Now;
                correlationId = Guid.NewGuid();
                operations = await GetUserOperationsAsync(correlationId).ConfigureAwait(false);

                principal = new CustomerPrincipal(ClaimsPrincipal.Current);

                if (principal.CustomerId.Equals(provider.Configuration.PartnerCenterApplicationTenantId) ||
                    principal.CustomerId.Equals(customerId))
                {
                    await operations.Customers.ById(customerId).Users.ById(userId).LicenseUpdates.CreateAsync(entity).ConfigureAwait(false);
                }
                else
                {
                    throw new UnauthorizedAccessException("You are not authorized to perform this operation.");
                }

                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(executionTime).TotalMilliseconds }
                };

                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", principal.CustomerId },
                    { "Name", principal.Name },
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                provider.Telemetry.TrackEvent(nameof(UpdateUserLicensesAsync), eventProperties, eventMetrics);
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
        private async Task<IPartner> GetAppOperationsAsync(Guid correlationId)
        {
            if (appOperations == null || appOperations.Credentials.ExpiresAt > DateTime.UtcNow)
            {
                IPartnerCredentials credentials = await GetPartnerCenterCredentialsAsync().ConfigureAwait(false);

                lock (appLock)
                {
                    appOperations = PartnerService.Instance.CreatePartnerOperations(credentials);
                }

                PartnerService.Instance.ApplicationName = ApplicationName;
            }

            // TODO -- Add localization
            // return appOperations.With(RequestContextFactory.Instance.Create(correlationId, service.Localization.Locale));
            return appOperations.With(RequestContextFactory.Instance.Create(correlationId));
        }

        /// <summary>
        /// Gets an instance of <see cref="IPartnerCredentials"/> used to access the Partner Center Managed API.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <returns>
        /// An instance of <see cref="IPartnerCredentials" /> that represents the access token.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="authority"/> is empty or null.
        /// </exception>
        private async Task<IPartnerCredentials> GetPartnerCenterCredentialsAsync()
        {
            // Attempt to obtain the Partner Center token from the cache.
            IPartnerCredentials credentials =
                 await provider.Cache.FetchAsync<Models.PartnerCenterToken>(
                     CacheDatabaseType.Authentication, PartnerCenterCacheKey).ConfigureAwait(false);

            if (credentials != null && !credentials.IsExpired())
            {
                return credentials;
            }

            // The access token has expired, so a new one must be requested.
            credentials = await PartnerCredentials.Instance.GenerateByApplicationCredentialsAsync(
                provider.Configuration.PartnerCenterApplicationId,
                provider.Configuration.PartnerCenterApplicationSecret.ToUnsecureString(),
                provider.Configuration.PartnerCenterApplicationTenantId).ConfigureAwait(false);

            await provider.Cache.StoreAsync(CacheDatabaseType.Authentication, PartnerCenterCacheKey, credentials).ConfigureAwait(false);

            return credentials;
        }

        /// <summary>
        /// Gets an instance of the partner service that utilizes app plus user authentication.
        /// </summary>
        /// <param name="correlationId">Correlation identifier for the operation.</param>
        /// <returns>An instance of the partner service.</returns>
        private async Task<IPartner> GetUserOperationsAsync(Guid correlationId)
        {
            AuthenticationResult token = await provider.AccessToken.GetAccessTokenAsync(
                $"{provider.Configuration.ActiveDirectoryEndpoint}/{provider.Configuration.PartnerCenterApplicationTenantId}",
                provider.Configuration.PartnerCenterEndpoint,
                new Models.ApplicationCredential
                {
                    ApplicationId = provider.Configuration.ApplicationId,
                    ApplicationSecret = provider.Configuration.ApplicationSecret,
                    UseCache = true
                },
                provider.AccessToken.UserAssertionToken).ConfigureAwait(false);

            IPartnerCredentials credentials = await PartnerCredentials.Instance.GenerateByUserCredentialsAsync(
                provider.Configuration.ApplicationId,
                new AuthenticationToken(token.AccessToken, token.ExpiresOn)).ConfigureAwait(false);

            IAggregatePartner userOperations = PartnerService.Instance.CreatePartnerOperations(credentials);

            return userOperations.With(RequestContextFactory.Instance.Create(correlationId));
        }
    }
}