// -----------------------------------------------------------------------
// <copyright file="ServiceCommunications.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic.Office
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    /// <summary>
    /// Facilities interactions with the Office 365 Service Communications API.
    /// </summary>
    public class ServiceCommunications
    {
        /// <summary>
        /// Authentication token that should be utilized the requests.
        /// </summary>
        private readonly AuthenticationResult token;

        /// <summary>
        /// Provides access to core services.
        /// </summary>
        private readonly IExplorerService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCommunications" /> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        /// <param name="token">Access token to be used for the requests.</param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="token"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="service"/> is null.
        /// </exception>
        public ServiceCommunications(IExplorerService service, AuthenticationResult token)
        {
            service.AssertNotNull(nameof(service));
            this.service = service;
            this.token = token;
        }

        /// <summary>
        /// Gets the current status for the specified tenant.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <returns>A list of health events associated with the specified tenant identifier.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="customerId"/> is empty or null.
        /// </exception>
        public async Task<List<IHealthEvent>> GetCurrentStatusAsync(string customerId)
        {
            Result<OfficeHealthEvent> records;
            string requestUri;

            customerId.AssertNotEmpty(nameof(customerId));

            try
            {
                requestUri = $"{service.Configuration.OfficeManagementEndpoint}/api/v1.0/{customerId}/ServiceComms/CurrentStatus";

                records = await service.Communication.GetAsync<Result<OfficeHealthEvent>>(
                    requestUri,
                    token.AccessToken).ConfigureAwait(false);

                return records.Value.ToList<IHealthEvent>();
            }
            finally
            {
                records = null;
            }
        }
    }
}