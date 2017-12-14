// -----------------------------------------------------------------------
// <copyright file="Configuration.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Configuration
{
    using System;
    using System.Configuration;
    using System.Security;
    using System.Threading.Tasks;
    using Logic;

    /// <summary>
    /// Provides easy access to various configurations stored in sources like app.config and web.config
    /// </summary>
    /// <seealso cref="IConfiguration" />
    public class Configuration : IConfiguration
    {
        /// <summary>
        /// Provides access to core services.
        /// </summary>
        private readonly IExplorerService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="service"/> is null.
        /// </exception>
        public Configuration(IExplorerService service)
        {
            service.AssertNotNull(nameof(service));
            this.service = service;
        }

        /// <summary>
        /// Gets the Active Directory endpoint address.
        /// </summary>
        public string ActiveDirectoryEndpoint => ConfigurationManager.AppSettings["ActiveDirectoryEndpoint"];

        /// <summary>
        /// Gets the application identifier value.
        /// </summary>
        public string ApplicationId => ConfigurationManager.AppSettings["ApplicationId"];

        /// <summary>
        /// Gets the application secret value.
        /// </summary>
        public SecureString ApplicationSecret { get; private set; }

        /// <summary>
        /// Gets the application tenant identifier.
        /// </summary>
        public string ApplicationTenantId => ConfigurationManager.AppSettings["ApplicationTenantId"];

        /// <summary>
        /// Gets the Azure Resource Manager endpoint address.
        /// </summary>
        public string AzureResourceManagerEndpoint => ConfigurationManager.AppSettings["AzureResourceManagerEndpoint"];

        /// <summary>
        /// Gets the Microsoft Graph endpoint address.
        /// </summary>
        public string GraphEndpoint => ConfigurationManager.AppSettings["GraphEndpoint"];

        /// <summary>
        /// Gets the Application Insights instrumentation key.
        /// </summary>
        public string InstrumentationKey => ConfigurationManager.AppSettings["InstrumentationKey"];

        /// <summary>
        /// Gets the endpoint address for the instance of Key Vault.
        /// </summary>
        public string KeyVaultEndpoint => ConfigurationManager.AppSettings["KeyVaultEndpoint"];

        /// <summary>
        /// Gets a value indicating whether or not the reseller tenant is the TIP tenant.
        /// </summary>
        public bool IsIntegrationSandbox => Convert.ToBoolean(ConfigurationManager.AppSettings["IsIntegrationSandbox"]);

        /// <summary>
        /// Gets the Office 365 Management endpoint address.
        /// </summary>
        public string OfficeManagementEndpoint => ConfigurationManager.AppSettings["OfficeManagementEndpoint"];

        /// <summary>
        /// Gets the Partner Center application identifier.
        /// </summary>
        public string PartnerCenterApplicationId => ConfigurationManager.AppSettings["PartnerCenterApplicationId"];

        /// <summary>
        /// Gets the Partner Center application secret.
        /// </summary>
        public SecureString PartnerCenterApplicationSecret { get; private set; }

        /// <summary>
        /// Gets the Partner Center application tenant identifier.
        /// </summary>
        public string PartnerCenterApplicationTenantId => ConfigurationManager.AppSettings["PartnerCenterApplicationTenantId"];

        /// <summary>
        /// Gets the Partner Center endpoint address.
        /// </summary>
        public string PartnerCenterEndpoint => ConfigurationManager.AppSettings["PartnerCenterEndpoint"];

        /// <summary>
        /// Gets the Redis Cache connection string.
        /// </summary>
        public SecureString RedisCacheConnectionString { get; private set; }

        /// <summary>
        /// Performs the necessary initialization operations.
        /// </summary>
        /// <returns>An instance of the  <see cref="Task"/> class that represents the asynchronous operation.</returns>
        public async Task InitializeAsync()
        {
            ApplicationSecret = await service.Vault.GetAsync("ApplicationSecret");
            PartnerCenterApplicationSecret = await service.Vault.GetAsync("PartnerCenterApplicationSecret");
            RedisCacheConnectionString = await service.Vault.GetAsync("RedisCacheConnectionString");
        }
    }
}