// -----------------------------------------------------------------------
// <copyright file="DomainsController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Logic;
    using Microsoft.Graph;
    using Models;
    using Providers;
    using Security;

    /// <summary>
    /// Controller for all Domain views.
    /// </summary>
    [AuthorizationFilter(Roles = UserRole.Partner)]
    public class DomainsController : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainsController"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        public DomainsController(IExplorerProvider provider) : base(provider)
        {
        }

        /// <summary>
        /// Obtains the service configuration records for the specified domain. 
        /// </summary>
        /// <param name="domain">Name of the domain</param>
        /// <returns>A collection of service configuration records for the specified domain in JSON.</returns>
        public async Task<PartialViewResult> ConfigurationRecords(string customerId, string domain)
        {
            GraphClient client;
            List<DomainDnsRecord> records;

            try
            {
                client = new GraphClient(Provider, customerId);
                records = await client.GetDomainConfigurationRecordsAsync(domain).ConfigureAwait(false);

                return PartialView(new ConfigurationRecordsModel { ServiceConfigurationRecords = records });
            }
            finally
            {
                client = null;
                records = null;
            }
        }

        /// <summary>
        /// Determines whether the specified domain is available or not.
        /// </summary>
        /// <param name="primaryDomain">The domain prefix to be checked.</param>
        /// <returns><c>true</c> if the domain available; otherwise <c>false</c> is returned.</returns>
        /// <remarks>
        /// This checks if the specified domain is available using the Partner Center API. A domain is
        /// considered to be available if the domain is not already in used by another Azure AD tenant.
        /// </remarks>
        public async Task<JsonResult> IsDomainAvailable(string primaryDomain)
        {
            if (string.IsNullOrEmpty(primaryDomain))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            bool exists = await Provider.PartnerOperations.CheckDomainAsync($"{primaryDomain}.onmicrosoft.com").ConfigureAwait(false);

            return Json(!exists, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lists the domains for the specified customer identifier.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>A collection of domains that belong to the specified customer identifier.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="customerId"/> is empty or null.</exception>
        [HttpGet]
        public async Task<JsonResult> List(string customerId)
        {
            GraphClient client;
            List<DomainModel> domains;

            customerId.AssertNotEmpty(nameof(customerId));

            try
            {
                client = new GraphClient(Provider, customerId);
                domains = await client.GetDomainsAsync().ConfigureAwait(false);

                return Json(domains, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                client = null;
                domains = null;
            }
        }
    }
}