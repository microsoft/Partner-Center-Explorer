// -----------------------------------------------------------------------
// <copyright file="Configuration.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
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
        public string ApplicationSecret => this.GetConfigurationValue("ApplicationSecret");

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
        public string PartnerCenterApplicationSecret => this.GetConfigurationValue("PartnerCenterApplicationSecret");

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
        public string RedisCacheConnectionString => this.GetConfigurationValue("RedisCacheConnectionString");

        /// <summary>
        /// Gets the storage account key.
        /// </summary>
        public string StorageAccountKey => this.GetConfigurationValue("StorageAccountKey");

        /// <summary>
        /// Gets the vault application identifier.
        /// </summary>
        public string VaultApplicationId => ConfigurationManager.AppSettings["VaultApplicationId"];

        /// <summary>
        /// Gets the vault application certificate thumbprint.
        /// </summary>
        public string VaultApplicationCertThumbprint => ConfigurationManager.AppSettings["VaultApplicationCertThumbprint"];

        /// <summary>
        /// Gets the vault application tenant identifier.
        /// </summary>
        public string VaultApplicationTenantId => ConfigurationManager.AppSettings["VaultApplicationTenantId"];

        /// <summary>
        /// Gets the vault identifier value.
        /// </summary>
        public string VaultBaseAddress => ConfigurationManager.AppSettings["VaultBaseAddress"];

        /// <summary>
        /// Gets the configuration value.
        /// </summary>
        /// <param name="identifier">Identifier of the resource being requested.</param>
        /// <returns>A string represented the value of the configuration.</returns>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="identifier"/> is empty or null.
        /// </exception>
        private string GetConfigurationValue(string identifier)
        {
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            string value;

            identifier.AssertNotNull(nameof(identifier));

            try
            {
                startTime = DateTime.Now;

                value = this.service.Vault.Get(identifier);

                if (string.IsNullOrEmpty(value))
                {
                    value = ConfigurationManager.AppSettings[identifier];
                }

                // Track the event measurements for analysis.
                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds }
                };

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "Identifier", identifier }
                };

                this.service.Telemetry.TrackEvent("Configuration/GetConfigurationValue", eventProperties, eventMetrics);

                return value;
            }
            finally
            {
                eventMetrics = null;
                eventProperties = null;
            }
        }
    }
}