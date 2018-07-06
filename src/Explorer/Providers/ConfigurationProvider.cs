// -----------------------------------------------------------------------
// <copyright file="ConfigurationProvider.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Providers
{
    using System.Configuration;
    using System.Security;
    using System.Threading.Tasks;
    using Logic;

    /// <summary>
    /// Provides easy access to various configurations stored in sources like app.config and web.config
    /// </summary>
    /// <seealso cref="IConfigurationProvider" />
    public class ConfigurationProvider : IConfigurationProvider
    {
        /// <summary>
        /// Provides access to core explorer providers.
        /// </summary>
        private readonly IExplorerProvider service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationProvider"/> class.
        /// </summary>
        /// <param name="service">Provides access to core explorer providers.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is null.
        /// </exception>
        public ConfigurationProvider(IExplorerProvider service)
        {
            service.AssertNotNull(nameof(service));
            this.service = service;
        }

        /// <summary>
        /// Gets the Active Directory endpoint address.
        /// </summary>
        public string ActiveDirectoryEndpoint { get; private set; }

        /// <summary>
        /// Gets the application identifier value.
        /// </summary>
        public string ApplicationId { get; private set; }

        /// <summary>
        /// Gets the application secret value.
        /// </summary>
        public SecureString ApplicationSecret { get; private set; }

        /// <summary>
        /// Gets the application tenant identifier.
        /// </summary>
        public string ApplicationTenantId { get; private set; }

        /// <summary>
        /// Gets the Azure Resource Manager endpoint address.
        /// </summary>
        public string AzureResourceManagerEndpoint { get; private set; }

        /// <summary>
        /// Gets the Microsoft Graph endpoint address.
        /// </summary>
        public string GraphEndpoint { get; private set; }

        /// <summary>
        /// Gets the Application Insights instrumentation key.
        /// </summary>
        public string InstrumentationKey { get; private set; }

        /// <summary>
        /// Gets the endpoint address for the instance of Key Vault.
        /// </summary>
        public string KeyVaultEndpoint { get; private set; }

        /// <summary>
        /// Gets the Office 365 Management endpoint address.
        /// </summary>
        public string OfficeManagementEndpoint { get; private set; }

        /// <summary>
        /// Gets the Partner Center application tenant identifier.
        /// </summary>
        public string PartnerCenterAccountId { get; private set; }

        /// <summary>
        /// Gets the Partner Center application identifier.
        /// </summary>
        public string PartnerCenterApplicationId { get; private set; }

        /// <summary>
        /// Gets the Partner Center application secret.
        /// </summary>
        public SecureString PartnerCenterApplicationSecret { get; private set; }

        /// <summary>
        /// Gets the Partner Center endpoint address.
        /// </summary>
        public string PartnerCenterEndpoint { get; private set; }

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
            ActiveDirectoryEndpoint = ConfigurationManager.AppSettings["ActiveDirectoryEndpoint"];
            ApplicationId = ConfigurationManager.AppSettings["ApplicationId"];
            ApplicationTenantId = ConfigurationManager.AppSettings["ApplicationTenantId"];
            AzureResourceManagerEndpoint = ConfigurationManager.AppSettings["AzureResourceManagerEndpoint"];
            GraphEndpoint = ConfigurationManager.AppSettings["GraphEndpoint"];
            InstrumentationKey = ConfigurationManager.AppSettings["InstrumentationKey"];
            KeyVaultEndpoint = ConfigurationManager.AppSettings["KeyVaultEndpoint"];
            OfficeManagementEndpoint = ConfigurationManager.AppSettings["OfficeManagementEndpoint"];
            PartnerCenterAccountId = ConfigurationManager.AppSettings["PartnerCenterAccountId"];
            PartnerCenterApplicationId = ConfigurationManager.AppSettings["PartnerCenterApplicationId"];
            PartnerCenterEndpoint = ConfigurationManager.AppSettings["PartnerCenterEndpoint"];

            ApplicationSecret = await service.Vault.GetAsync("ApplicationSecret").ConfigureAwait(false);
            PartnerCenterApplicationSecret = await service.Vault.GetAsync("PartnerCenterApplicationSecret").ConfigureAwait(false);
            RedisCacheConnectionString = await service.Vault.GetAsync("RedisCacheConnectionString").ConfigureAwait(false);
        }
    }
}