// -----------------------------------------------------------------------
// <copyright file="GraphClient.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Graph;
    using Models;
    using Providers;
    using Security;

    /// <summary>
    /// Provides the ability to interact with the Microsoft Graph.
    /// </summary>
    /// <seealso cref="IGraphClient" />
    public class GraphClient : IGraphClient
    {
        /// <summary>
        /// Static instance of the <see cref="HttpProvider" /> class.
        /// </summary>
        private static HttpProvider httpProvider = new HttpProvider(new HttpClientHandler(), false);

        /// <summary>
        /// Provides access to core services.
        /// </summary>
        private readonly IExplorerProvider provider;

        /// <summary>
        /// Provides access to the Microsoft Graph.
        /// </summary>
        private readonly IGraphServiceClient client;

        /// <summary>
        /// Identifier of the customer.
        /// </summary>
        private readonly string customerId;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphClient"/> class.
        /// </summary>
        /// <param name="provider">Provides access to core services.</param>
        /// <param name="customerId">Identifier for customer whose resources are being accessed.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider"/> is null.
        /// </exception>
        public GraphClient(IExplorerProvider provider, string customerId)
        {
            provider.AssertNotNull(nameof(provider));
            customerId.AssertNotEmpty(nameof(customerId));

            this.customerId = customerId;
            this.provider = provider;

            client = new GraphServiceClient(new AuthenticationProvider(this.provider, customerId), httpProvider);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphClient"/> class.
        /// </summary>
        /// <param name="provider">Provides access to core services.</param>
        /// <param name="client">Provides the ability to interact with the Microsoft Graph.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider"/> is null.
        /// or
        /// <paramref name="client"/> is null.
        /// </exception>
        public GraphClient(IExplorerProvider provider, IGraphServiceClient client)
        {
            provider.AssertNotNull(nameof(provider));
            client.AssertNotNull(nameof(client));

            this.provider = provider;
            this.client = client;
        }

        /// <summary>
        /// Gets a list of roles assigned to the specified object identifier.
        /// </summary>
        /// <param name="objectId">Object identifier for the object to be checked.</param>
        /// <returns>A list of roles that that are associated with the specified object identifier.</returns>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="objectId"/> is empty or null.
        /// </exception>
        public async Task<List<RoleModel>> GetDirectoryRolesAsync(string objectId)
        {
            DateTime executionTime;
            Dictionary<string, double> eventMeasurements;
            Dictionary<string, string> eventProperties;
            IUserMemberOfCollectionWithReferencesPage directoryGroups;
            List<RoleModel> roles;
            List<DirectoryRole> directoryRoles;
            List<Group> groups;
            bool morePages;

            objectId.AssertNotEmpty(nameof(objectId));

            try
            {
                executionTime = DateTime.Now;

                directoryGroups = await client.Users[objectId].MemberOf.Request().GetAsync().ConfigureAwait(false);
                roles = new List<RoleModel>();

                do
                {
                    directoryRoles = directoryGroups.CurrentPage.OfType<DirectoryRole>().ToList();

                    if (directoryRoles.Count > 0)
                    {
                        roles.AddRange(directoryRoles.Select(r => new RoleModel
                        {
                            Description = r.Description,
                            DisplayName = r.DisplayName
                        }));
                    }

                    if (customerId.Equals(provider.Configuration.PartnerCenterAccountId, StringComparison.InvariantCultureIgnoreCase))
                    {
                        groups = directoryGroups.CurrentPage.OfType<Group>().Where(
                            g => g.DisplayName.Equals("AdminAgents", StringComparison.InvariantCultureIgnoreCase)
                                || g.DisplayName.Equals("HelpdeskAgents", StringComparison.InvariantCultureIgnoreCase)
                                || g.DisplayName.Equals("SalesAgent", StringComparison.InvariantCultureIgnoreCase)).ToList();

                        if (groups.Count > 0)
                        {
                            roles.AddRange(groups.Select(g => new RoleModel
                            {
                                Description = g.Description,
                                DisplayName = g.DisplayName
                            }));
                        }
                    }

                    morePages = directoryGroups.NextPageRequest != null;

                    if (morePages)
                    {
                        directoryGroups = await directoryGroups.NextPageRequest.GetAsync().ConfigureAwait(false);
                    }
                }
                while (morePages);

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "CustomerId", customerId },
                    { "ObjectId", objectId }
                };

                // Track the event measurements for analysis.
                eventMeasurements = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(executionTime).TotalMilliseconds },
                    { "NumberOfRoles", roles.Count }
                };

                provider.Telemetry.TrackEvent(nameof(GetDirectoryRolesAsync), eventProperties, eventMeasurements);

                return roles;
            }
            catch (Exception ex)
            {
                provider.Telemetry.TrackException(ex);
                return null;
            }
            finally
            {
                directoryGroups = null;
                directoryRoles = null;
                eventMeasurements = null;
                eventProperties = null;
                groups = null;
            }
        }

        /// <summary>
        /// Gets the service configuration records for the specified domain. 
        /// </summary>
        /// <param name="domain">Name of the domain</param>
        /// <returns>A list of service configuration records for the specified domain.</returns>
        public async Task<List<DomainDnsRecord>> GetDomainConfigurationRecordsAsync(string domain)
        {
            IDomainServiceConfigurationRecordsCollectionPage records;

            try
            {
                records = await client.Domains[domain].ServiceConfigurationRecords.Request().GetAsync().ConfigureAwait(false);
                return records.CurrentPage.ToList();
            }
            finally
            {
                records = null;
            }
        }

        /// <summary>
        /// Gets a list of domains configured for the customer.
        /// </summary>
        /// <returns>A list of domains configured for the customer.</returns>
        public async Task<List<DomainModel>> GetDomainsAsync()
        {
            IGraphServiceDomainsCollectionPage domains;
            List<DomainModel> models;
            bool morePages;

            try
            {
                domains = await client.Domains.Request().GetAsync().ConfigureAwait(false);
                models = new List<DomainModel>();

                do
                {
                    models.AddRange(domains.CurrentPage.Select(d => new DomainModel
                    {
                        AdminManaged = d.IsAdminManaged.Value,
                        AuthenticationType = d.AuthenticationType,
                        AvailabilityStatus = d.AvailabilityStatus,
                        IsDefault = d.IsDefault.Value,
                        IsInitial = d.IsInitial.Value,
                        IsRoot = d.IsRoot.Value,
                        IsVerified = d.IsVerified.Value,
                        Name = d.Id,
                        SupportedServices = d.SupportedServices
                    }));

                    morePages = domains.NextPageRequest != null;

                    if (morePages)
                    {
                        domains = await domains.NextPageRequest.GetAsync().ConfigureAwait(false);
                    }
                }
                while (morePages);

                return models;
            }
            finally
            {
                domains = null;
            }
        }

        /// <summary>
        /// Gets a list of users for the customer.
        /// </summary>
        /// <returns>A list of users that belong to the customer.</returns>
        public async Task<List<UserModel>> GetUsersAsync()
        {
            IGraphServiceUsersCollectionPage users;
            List<UserModel> value;
            bool morePages;

            try
            {
                users = await client.Users.Request().GetAsync().ConfigureAwait(false);
                value = new List<UserModel>();

                do
                {
                    value.AddRange(users.CurrentPage.Select(u => new UserModel()
                    {
                        CustomerId = customerId,
                        DisplayName = u.DisplayName,
                        FirstName = u.GivenName,
                        Id = u.Id,
                        LastName = u.Surname,
                        UsageLocation = u.UsageLocation,
                        UserPrincipalName = u.UserPrincipalName
                    }));

                    morePages = users.NextPageRequest != null;

                    if (morePages)
                    {
                        users = await users.NextPageRequest.GetAsync().ConfigureAwait(false);
                    }
                }
                while (morePages);

                return value;
            }
            finally
            {
                users = null;
            }
        }
    }
}