// -----------------------------------------------------------------------
// <copyright file="ResourceManager.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic.Azure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.Management.ResourceManager;
    using Microsoft.Azure.Management.ResourceManager.Models;
    using Providers;
    using Rest;
    using Rest.Azure;

    /// <summary>
    /// Facilitates interactions with the Azure Resource Manager API.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class ResourceManager : IDisposable
    {
        /// <summary>
        /// Provides access to core services.
        /// </summary>
        private readonly IExplorerProvider provider;

        /// <summary>
        /// Provides the ability to interact with the Azure Resource Manager API.
        /// </summary>
        private ResourceManagementClient client;

        /// <summary>
        /// A flag indicating whether or not this object has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceManager"/> class.
        /// </summary>
        /// <param name="provider">Provides access to core services.</param>
        /// <param name="token">A valid JSON Web Token (JWT).</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="token"/> is empty or null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider"/> is null.
        /// </exception>
        public ResourceManager(IExplorerProvider provider, string token)
        {
            provider.AssertNotNull(nameof(provider));
            token.AssertNotEmpty(nameof(token));

            client = new ResourceManagementClient(new TokenCredentials(token));
            this.provider = provider;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Apply an Azure Resource Manager (ARM) template.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="resourceGroupName">Name of the resource group.</param>
        /// <param name="templateUri">URI for the ARM template.</param>
        /// <param name="parametersUri">URI for the ARM template parameters.</param>
        /// <returns>A <see cref="string"/> indicating the provisioning status.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="subscriptionId"/> is empty or null.
        /// or
        /// <paramref name="resourceGroupName"/> is empty or null.
        /// or
        /// <paramref name="templateUri"/> is empty or null.
        /// </exception>
        public async Task<string> ApplyTemplateAsync(string subscriptionId, string resourceGroupName, Uri templateUri, Uri parametersUri)
        {
            Deployment deployment;
            DeploymentExtended result;
            string deploymentName;

            subscriptionId.AssertNotEmpty(nameof(subscriptionId));
            resourceGroupName.AssertNotEmpty(nameof(resourceGroupName));

            try
            {
                client.SubscriptionId = subscriptionId;

                deployment = new Deployment()
                {
                    Properties = new DeploymentProperties()
                    {
                        Mode = DeploymentMode.Incremental,
                        TemplateLink = new TemplateLink(templateUri.ToString())
                    }
                };

                if (parametersUri != null)
                {
                    deployment.Properties.ParametersLink = new ParametersLink(parametersUri.ToString());
                }

                deploymentName = Guid.NewGuid().ToString();

                result = await client.Deployments.CreateOrUpdateAsync(
                    resourceGroupName,
                    deploymentName,
                    deployment).ConfigureAwait(false);

                return result.Properties.ProvisioningState;
            }
            finally
            {
                deployment = null;
                result = null;
            }
        }

        /// <summary>
        /// Gets a list of deployments for the specified subscription and resource group.
        /// </summary>
        /// <param name="subscriptionId">Identifier for the subscription.</param>
        /// <param name="resourceGroupName">Name of the resource group.</param>
        /// <returns>A list of deployments for the specified subscriptions and resource group.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="subscriptionId"/> is empty or null.
        /// or
        /// <paramref name="resourceGroupName"/> is empty or null.
        /// </exception>
        public async Task<List<DeploymentExtended>> GetDeploymentsAsync(string subscriptionId, string resourceGroupName)
        {
            IPage<DeploymentExtended> deployements;

            subscriptionId.AssertNotEmpty(nameof(subscriptionId));
            resourceGroupName.AssertNotEmpty(nameof(resourceGroupName));

            try
            {
                client.SubscriptionId = subscriptionId;
                deployements = await client.Deployments.ListByResourceGroupAsync(resourceGroupName).ConfigureAwait(false);

                return deployements.ToList();
            }
            finally
            {
                deployements = null;
            }
        }

        /// <summary>
        /// Gets a collection of resource groups for the specified subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns>A collection of resource groups.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="subscriptionId"/> is empty or null. 
        /// </exception>
        public async Task<List<ResourceGroup>> GetResourceGroupsAsync(string subscriptionId)
        {
            IPage<ResourceGroup> resourceGroups;

            subscriptionId.AssertNotEmpty(nameof(subscriptionId));

            try
            {
                client.SubscriptionId = subscriptionId;
                resourceGroups = await client.ResourceGroups.ListAsync().ConfigureAwait(false);
                return resourceGroups.ToList();
            }
            finally
            {
                resourceGroups = null;
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                client.Dispose();
            }

            disposed = true;
        }
    }
}