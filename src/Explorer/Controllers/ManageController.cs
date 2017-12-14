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
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
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
                customer = await Service.PartnerOperations.GetCustomerAsync(customerId);
                subscription = await Service.PartnerOperations.GetSubscriptionAsync(customerId, subscriptionId);

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

                return View(manageModel.ViewName, manageModel);
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

            return PartialView(newDeploymentModel);
        }

        /// <summary>
        /// Creates a new Azure Resource Manager (ARM) deployment.
        /// </summary>
        /// <param name="model">An instance of <see cref="NewDeploymentModel"/>.</param>
        /// <returns>A collection of deployments</returns>
        [HttpPost]
        public async Task<PartialViewResult> NewDeployment(NewDeploymentModel model)
        {
            AuthenticationResult token;
            List<DeploymentModel> results;

            try
            {
                token = await GetAccessTokenAsync(
                    $"{Service.Configuration.ActiveDirectoryEndpoint}/{model.CustomerId}");

                using (ResourceManager manager = new ResourceManager(Service, token.AccessToken))
                {
                    await manager.ApplyTemplateAsync(
                        model.SubscriptionId,
                        model.ResourceGroupName,
                        model.TemplateUri,
                        model.ParametersUri);

                    results = await GetDeploymentsAsync(model.CustomerId, model.ResourceGroupName, model.SubscriptionId);
                    return PartialView("Deployments", results);
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
            AuthenticationResult token;
            List<ResourceGroup> groups;

            customerId.AssertNotEmpty(nameof(customerId));
            subscriptionId.AssertNotEmpty(nameof(subscriptionId));

            try
            {
                token = await GetAccessTokenAsync(
                    $"{Service.Configuration.ActiveDirectoryEndpoint}/{customerId}");

                using (ResourceManager manager = new ResourceManager(Service, token.AccessToken))
                {
                    groups = await manager.GetResourceGroupsAsync(subscriptionId);
                    return Json(groups, JsonRequestBehavior.AllowGet);
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
            AuthenticationResult token;
            List<DeploymentExtended> deployments;

            customerId.AssertNotEmpty(nameof(customerId));
            resourceGroupName.AssertNotEmpty(nameof(resourceGroupName));
            subscriptionId.AssertNotEmpty(nameof(subscriptionId));

            try
            {
                token = await GetAccessTokenAsync(
                  $"{Service.Configuration.ActiveDirectoryEndpoint}/{customerId}");

                using (ResourceManager manager = new ResourceManager(Service, token.AccessToken))
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

        private async Task<AuthenticationResult> GetAccessTokenAsync(string authority)
        {
            return await Service.AccessToken.GetAccessTokenAsync(
                authority,
                Service.Configuration.AzureResourceManagerEndpoint,
                new ApplicationCredential
                {
                    ApplicationId = Service.Configuration.ApplicationId,
                    ApplicationSecret = Service.Configuration.ApplicationSecret,
                    UseCache = true
                },
                Service.AccessToken.UserAssertionToken);
        }
    }
}