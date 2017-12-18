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
    using System.Threading.Tasks;
    using Microsoft.Azure.ActiveDirectory.GraphClient;
    using Microsoft.Azure.ActiveDirectory.GraphClient.Extensions;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Models;

    /// <summary>
    /// Provides the ability to interact with the Microsoft Graph.
    /// </summary>
    /// <seealso cref="IGraphClient" />
    public class GraphClient : IGraphClient
    {
        /// <summary>
        /// Provides access to core services.
        /// </summary>
        private readonly IExplorerService service;

        /// <summary>
        /// Provides access to the Microsoft Azure AD Graph API.
        /// </summary>
        private readonly IActiveDirectoryClient client;

        /// <summary>
        /// Identifier of the customer.
        /// </summary>
        private readonly string customerId;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphClient"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        /// <param name="customerId">Identifier for customer whose resources are being accessed.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is null.
        /// </exception>
        public GraphClient(IExplorerService service, string customerId)
        {
            service.AssertNotNull(nameof(service));
            customerId.AssertNotEmpty(nameof(customerId));

            this.customerId = customerId;
            this.service = service;

            client = new ActiveDirectoryClient(
                new Uri($"{this.service.Configuration.GraphEndpoint}/{customerId}"),
                async () =>
                {
                    AuthenticationResult token = await service.AccessToken.GetAccessTokenAsync(
                        $"{this.service.Configuration.ActiveDirectoryEndpoint}/{customerId}",
                        service.Configuration.GraphEndpoint,
                        new ApplicationCredential
                        {
                            ApplicationId = service.Configuration.ApplicationId,
                            ApplicationSecret = service.Configuration.ApplicationSecret,
                            UseCache = true
                        }).ConfigureAwait(false);

                    return token.AccessToken;
                });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphClient"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        /// <param name="client">Provides the ability to interact with the Microsoft AD Graph API.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is null.
        /// or
        /// <paramref name="client"/> is null.
        /// </exception>
        public GraphClient(IExplorerService service, IActiveDirectoryClient client)
        {
            service.AssertNotNull(nameof(service));
            client.AssertNotNull(nameof(client));

            this.service = service;
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
            IPagedCollection<IDirectoryObject> memberships;
            List<Group> groups;
            List<DirectoryRole> directoryRoles;
            List<RoleModel> roles;

            objectId.AssertNotEmpty(nameof(objectId));

            try
            {
                memberships = await client.Users.GetByObjectId(objectId).MemberOf.ExecuteAsync().ConfigureAwait(false);
                roles = new List<RoleModel>();

                do
                {
                    directoryRoles = memberships.CurrentPage.OfType<DirectoryRole>().ToList();

                    if (directoryRoles.Count > 0)
                    {
                        roles.AddRange(directoryRoles.Select(r => new RoleModel
                        {
                            Description = r.Description,
                            DisplayName = r.DisplayName
                        }));
                    }

                    if (customerId.Equals(service.Configuration.PartnerCenterApplicationTenantId))
                    {
                        groups = memberships.CurrentPage.OfType<Group>().Where(
                            g => g.DisplayName.Equals("AdminAgents") || g.DisplayName.Equals("HelpdeskAgents")).ToList();

                        if (groups.Count > 0)
                        {
                            roles.AddRange(groups.Select(g => new RoleModel
                            {
                                DisplayName = g.DisplayName
                            }));
                        }
                    }

                    memberships = await memberships.GetNextPageAsync().ConfigureAwait(false);
                }
                while (memberships != null);

                return roles;
            }
            finally
            {
                directoryRoles = null;
                memberships = null;
            }
        }

        /// <summary>
        /// Gets a list of domains configured for the customer.
        /// </summary>
        /// <returns>A list of domains configured for the customer.</returns>
        public async Task<List<DomainModel>> GetDomainsAsync()
        {
            IPagedCollection<IDomain> domains;
            List<DomainModel> models;

            try
            {
                domains = await client.Domains.ExecuteAsync().ConfigureAwait(false);
                models = new List<DomainModel>();

                do
                {
                    models.AddRange(domains.CurrentPage.Select(d => new DomainModel
                    {
                        AdminManaged = d.IsAdminManaged,
                        AuthenticationType = d.AuthenticationType,
                        AvailabilityStatus = d.AvailabilityStatus,
                        IsDefault = d.IsDefault,
                        IsInitial = d.IsInitial,
                        IsRoot = d.IsRoot,
                        IsVerified = d.IsVerified,
                        Name = d.Name,
                        ServiceConfigurationRecords = d.ServiceConfigurationRecords.CurrentPage.ToList(),
                        SupportedServices = d.SupportedServices
                    }));

                    domains = await domains.GetNextPageAsync().ConfigureAwait(false);
                }
                while (domains != null);

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
            IPagedCollection<IUser> users;
            List<UserModel> value;

            try
            {
                users = await client.Users.ExecuteAsync().ConfigureAwait(false);
                value = new List<UserModel>();

                do
                {
                    value.AddRange(users.CurrentPage.Select(u => new UserModel()
                    {
                        CustomerId = customerId,
                        DisplayName = u.DisplayName,
                        FirstName = u.GivenName,
                        Id = u.ObjectId,
                        LastName = u.Surname,
                        UsageLocation = u.UsageLocation,
                        UserPrincipalName = u.UserPrincipalName
                    }));

                    users = await users.GetNextPageAsync().ConfigureAwait(false);
                }
                while (users != null);

                return value;
            }
            finally
            {
                users = null;
            }
        }
    }
}