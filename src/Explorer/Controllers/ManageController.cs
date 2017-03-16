// -----------------------------------------------------------------------
// <copyright file="ManageController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Azure.Management.ResourceManager.Models;
    using Logic;
    using Logic.Azure;
    using Models;
    using PartnerCenter.Models.Customers;
    using PartnerCenter.Models.Invoices;
    using Security;
    using Subscriptions;

    /// <summary>
    /// Handles requests for the Manage views.
    /// </summary>
    [AuthorizationFilter(Roles = UserRole.Partner)]
    public class ManageController : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManageController"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        public ManageController(IExplorerService service) : base(service)
        {
        }

        /// <summary>
        /// Gets a list of deployments for the specified resource group.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="resourceGroupName">Name of the resource group.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns>The HTML template for the deployments view.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// or
        /// <paramref name="resourceGroupName"/> is empty or null.
        /// or
        /// <paramref name="subscriptionId"/> is empty or null.
        /// </exception>
        [HttpGet]
        public async Task<PartialViewResult> Deployments(string customerId, string resourceGroupName, string subscriptionId)
        {
            List<DeploymentModel> deployments;

            customerId.AssertNotEmpty(nameof(customerId));
            resourceGroupName.AssertNotEmpty(nameof(resourceGroupName));
            subscriptionId.AssertNotEmpty(nameof(subscriptionId));

            try
            {
                deployments = await this.GetDeploymentsAsync(customerId, resourceGroupName, subscriptionId);
                return this.PartialView("Deployments", deployments);
            }
            finally
            {
                deployments = null;
            }
        }

        /// <summary>
        /// Handles the Index view request.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns>The HTML template for the index page.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// or
        /// <paramref name="subscriptionId"/> is empty or null.
        /// </exception>
        public async Task<ActionResult> Index(string customerId, string subscriptionId)
        {
            Customer customer;
            SubscriptionManageModel manageModel;
            PartnerCenter.Models.Subscriptions.Subscription subscription;

            customerId.AssertNotEmpty(nameof(customerId));
            subscriptionId.AssertNotEmpty(nameof(subscriptionId));

            try
            {
                customer = await this.Service.PartnerCenter.Customers.ById(customerId).GetAsync();
                subscription = await this.Service.PartnerCenter.Customers.ById(customerId).Subscriptions.ById(subscriptionId).GetAsync();

                manageModel = new SubscriptionManageModel
                {
                    CompanyName = customer.CompanyProfile.CompanyName,
                    CustomerId = customer.Id,
                    FriendlyName = subscription.FriendlyName,
                    SubscriptionId = subscriptionId,
                    ViewName = (subscription.BillingType == BillingType.License) ? "Office" : "Azure"
                };

                if (manageModel.ViewName.Equals("Azure", StringComparison.CurrentCultureIgnoreCase))
                {
                    manageModel.SubscriptionDetails = new AzureSubscriptionDetails()
                    {
                        FriendlyName = subscription.FriendlyName,
                        Status = subscription.Status.ToString()
                    };
                }
                else
                {
                    manageModel.SubscriptionDetails = new OfficeSubscriptionDetails()
                    {
                        FriendlyName = subscription.FriendlyName,
                        Quantity = subscription.Quantity,
                        Status = subscription.Status.ToString()
                    };
                }

                return this.View(manageModel.ViewName, manageModel);
            }
            finally
            {
                customer = null;
                subscription = null;
            }
        }

        /// <summary>
        /// Handles the request to create a new deployment.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="resourceGroupName">Name of the resource group.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns>A partial view that will be used to configure the required values for the new deployment.</returns>
        [HttpGet]
        public PartialViewResult NewDeployment(string customerId, string resourceGroupName, string subscriptionId)
        {
            NewDeploymentModel newDeploymentModel = new NewDeploymentModel()
            {
                CustomerId = customerId,
                ResourceGroupName = resourceGroupName,
                SubscriptionId = subscriptionId
            };

            return this.PartialView(newDeploymentModel);
        }

        /// <summary>
        /// Creates a new Azure Resource Manager (ARM) deployment.
        /// </summary>
        /// <param name="model">An instance of <see cref="NewDeploymentModel"/>.</param>
        /// <returns>A collection of deployments</returns>
        [HttpPost]
        public async Task<PartialViewResult> NewDeployment(NewDeploymentModel model)
        {
            AuthenticationToken token;
            List<DeploymentModel> results;

            try
            {
                token = await this.Service.TokenManagement.GetAppPlusUserTokenAsync(
                    $"{this.Service.Configuration.ActiveDirectoryEndpoint}/{model.CustomerId}",
                   this.Service.Configuration.AzureResourceManagerEndpoint);

                using (ResourceManager manager = new ResourceManager(this.Service, token.Token))
                {
                    await manager.ApplyTemplateAsync(
                        model.SubscriptionId,
                        model.ResourceGroupName,
                        model.TemplateUri,
                        model.ParametersUri);

                    results = await this.GetDeploymentsAsync(model.CustomerId, model.ResourceGroupName, model.SubscriptionId);
                    return this.PartialView("Deployments", results);
                }
            }
            finally
            {
                results = null;
                token = null;
            }
        }

        /// <summary>
        /// Gets resource groups that belong to the specified customer and subscription.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns>Returns a collection of resource groups in JSON.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// or
        /// <paramref name="subscriptionId"/> is empty or null.
        /// </exception>
        [HttpGet]
        public async Task<JsonResult> ResourceGroups(string customerId, string subscriptionId)
        {
            AuthenticationToken token;
            List<ResourceGroup> groups;

            customerId.AssertNotEmpty(nameof(customerId));
            subscriptionId.AssertNotEmpty(nameof(subscriptionId));

            try
            {
                token = await this.Service.TokenManagement.GetAppPlusUserTokenAsync(
                    $"{this.Service.Configuration.ActiveDirectoryEndpoint}/{customerId}",
                   this.Service.Configuration.AzureResourceManagerEndpoint);

                using (ResourceManager manager = new ResourceManager(this.Service, token.Token))
                {
                    groups = await manager.GetResourceGroupsAsync(subscriptionId);
                    return this.Json(groups, JsonRequestBehavior.AllowGet);
                }
            }
            finally
            {
                groups = null;
                token = null;
            }
        }

        /// <summary>
        /// Gets a list of deployments for the specified resource group.
        /// </summary>
        /// <param name="customerId">Identifier of the customer.</param>
        /// <param name="resourceGroupName">Name of the resource group.</param>
        /// <param name="subscriptionId">Identifier of the subscription.</param>
        /// <returns>A list of <see cref="DeploymentModel"/>s that represents the deployments.</returns>
        private async Task<List<DeploymentModel>> GetDeploymentsAsync(string customerId, string resourceGroupName, string subscriptionId)
        {
            AuthenticationToken token;
            List<DeploymentExtended> deployments;

            customerId.AssertNotEmpty(nameof(customerId));
            resourceGroupName.AssertNotEmpty(nameof(resourceGroupName));
            subscriptionId.AssertNotEmpty(nameof(subscriptionId));

            try
            {
                token = await this.Service.TokenManagement.GetAppPlusUserTokenAsync(
                  $"{this.Service.Configuration.ActiveDirectoryEndpoint}/{customerId}",
                 this.Service.Configuration.AzureResourceManagerEndpoint);

                using (ResourceManager manager = new ResourceManager(this.Service, token.Token))
                {
                    deployments = await manager.GetDeploymentsAsync(subscriptionId, resourceGroupName);

                    return deployments.Select(d => new DeploymentModel()
                    {
                        Id = d.Id,
                        Name = d.Name,
                        ProvisioningState = d.Properties.ProvisioningState,
                        Timestamp = d.Properties.Timestamp.Value
                    }).ToList();
                }
            }
            finally
            {
                deployments = null;
                token = null;
            }
        }
    }
}