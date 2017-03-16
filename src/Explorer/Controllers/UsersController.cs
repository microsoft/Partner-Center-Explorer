// -----------------------------------------------------------------------
// <copyright file="UsersController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Logic;
    using Models;
    using PartnerCenter.Models;
    using PartnerCenter.Models.Licenses;
    using PartnerCenter.Models.Users;
    using Security;

    /// <summary>
    /// Controller for Users views.
    /// </summary>
    [AuthorizationFilter(Roles = UserRole.Partner)]
    public class UsersController : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        public UsersController(IExplorerService service) : base(service)
        {
        }

        /// <summary>
        /// Handles the Create view request.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>A partial view containing the NewUserModel model.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="customerId"/> is empty or null.
        /// </exception>
        [HttpGet]
        public PartialViewResult Create(string customerId)
        {
            customerId.AssertNotEmpty(nameof(customerId));

            NewUserModel newUserModel = new NewUserModel()
            {
                CustomerId = customerId
            };

            return this.PartialView(newUserModel);
        }

        /// <summary>
        /// Handles the Create user HTTP post.
        /// </summary>
        /// <param name="newUserModel">The new user model.</param>
        /// <returns>A partial view containing a list of users.</returns>
        [HttpPost]
        public async Task<PartialViewResult> Create(NewUserModel newUserModel)
        {
            CustomerUser customerUser;
            IGraphClient client;

            try
            {
                customerUser = new CustomerUser()
                {
                    DisplayName = newUserModel.DisplayName,
                    FirstName = newUserModel.FirstName,
                    LastName = newUserModel.LastName,
                    PasswordProfile = new PasswordProfile()
                    {
                        ForceChangePassword = false,
                        Password = newUserModel.Password
                    },
                    UsageLocation = newUserModel.UsageLocation,
                    UserPrincipalName = newUserModel.UserPrincipalName
                };

                client = new GraphClient(this.Service, newUserModel.CustomerId);

                await this.Service.PartnerCenter.Customers.ById(newUserModel.CustomerId).Users.CreateAsync(customerUser);

                UsersModel usersModel = new UsersModel()
                {
                    CustomerId = newUserModel.CustomerId,
                    Users = await client.GetUsersAsync()
                };

                return this.PartialView("List", usersModel);
            }
            finally
            {
                client = null;
                customerUser = null;
            }
        }

        /// <summary>
        /// Deletes the specified user.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>A HTTP OK if the delete is successful.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// or
        /// <paramref name="userId"/> is empty or null.
        /// </exception>
        [HttpDelete]
        public async Task<HttpResponseMessage> Delete(string customerId, string userId)
        {
            customerId.AssertNotEmpty(nameof(customerId));
            userId.AssertNotEmpty(nameof(userId));

            await this.Service.PartnerCenter.Customers.ById(customerId).Users.ById(userId).DeleteAsync();

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        /// <summary>
        /// Edits the specified user.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>A partial view containing the EditUserModel model.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// or
        /// <paramref name="userId"/> is empty or null.
        /// </exception>
        [HttpGet]
        public async Task<PartialViewResult> Edit(string customerId, string userId)
        {
            CustomerUser customerUser;
            EditUserModel editUserModel;

            customerId.AssertNotEmpty(nameof(customerId));
            userId.AssertNotEmpty(nameof(userId));

            try
            {
                customerUser = await this.Service.PartnerCenter.Customers.ById(customerId).Users.ById(userId).GetAsync();

                editUserModel = new EditUserModel()
                {
                    CustomerId = customerId,
                    DisplayName = customerUser.DisplayName,
                    FirstName = customerUser.FirstName,
                    LastName = customerUser.LastName,
                    Licenses = await this.GetLicenses(customerId, userId),
                    UsageLocation = "US",
                    UserId = userId,
                    UserPrincipalName = customerUser.UserPrincipalName
                };

                return this.PartialView(editUserModel);
            }
            finally
            {
                customerUser = null;
            }
        }

        /// <summary>
        /// Edits the specified user.
        /// </summary>
        /// <param name="editUserModel">The edit user model.</param>
        /// <returns>A list of users.</returns>
        [HttpPost]
        public async Task<PartialViewResult> Edit(EditUserModel editUserModel)
        {
            CustomerUser customerUser;
            IGraphClient client;

            try
            {
                client = new GraphClient(this.Service, editUserModel.CustomerId);
                customerUser = await this.Service.PartnerCenter.Customers.ById(editUserModel.CustomerId)
                    .Users.ById(editUserModel.UserId).GetAsync();

                customerUser.DisplayName = editUserModel.DisplayName;
                customerUser.FirstName = editUserModel.FirstName;
                customerUser.LastName = editUserModel.LastName;
                customerUser.UserPrincipalName = editUserModel.UserPrincipalName;
                customerUser.UsageLocation = editUserModel.UsageLocation;

                await this.Service.PartnerCenter.Customers.ById(editUserModel.CustomerId)
                    .Users.ById(editUserModel.UserId).PatchAsync(customerUser);

                await this.ProcessLicenseModifications(editUserModel);

                UsersModel usersModel = new UsersModel()
                {
                    CustomerId = editUserModel.CustomerId,
                    Users = await client.GetUsersAsync()
                };

                return this.PartialView("List", usersModel);
            }
            finally
            {
                client = null;
                customerUser = null;
            }
        }

        /// <summary>
        /// Lists the users that belong to the specified customer identifier.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>The HTML template for the list page.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// </exception>
        public async Task<PartialViewResult> List(string customerId)
        {
            IGraphClient client;

            customerId.AssertNotEmpty(nameof(customerId));

            try
            {
                client = new GraphClient(this.Service, customerId);

                UsersModel usersModel = new UsersModel()
                {
                    CustomerId = customerId,
                    Users = await client.GetUsersAsync()
                };

                return this.PartialView(usersModel);
            }
            finally
            {
                client = null;
            }
        }

        /// <summary>
        /// Gets the licenses assigned to the specified user.
        /// </summary>
        /// <param name="customerId">Identifier of the customer.</param>
        /// <param name="userId">Identifier of the user.</param>
        /// <returns>A list of licenses assigned to the user.</returns>
        private async Task<List<LicenseModel>> GetLicenses(string customerId, string userId)
        {
            LicenseModel licenseModel;
            List<LicenseModel> values;
            ResourceCollection<License> licenses;
            ResourceCollection<SubscribedSku> subscribedSkus;

            customerId.AssertNotEmpty(nameof(customerId));
            userId.AssertNotEmpty(nameof(userId));

            try
            {
                values = new List<LicenseModel>();
                licenses = await this.Service.PartnerCenter.Customers.ById(customerId).Users.ById(userId).Licenses.GetAsync();
                subscribedSkus = await this.Service.PartnerCenter.Customers.ById(customerId).SubscribedSkus.GetAsync();

                foreach (SubscribedSku sku in subscribedSkus.Items)
                {
                    licenseModel = new LicenseModel()
                    {
                        ConsumedUnits = sku.ConsumedUnits,
                        Id = sku.ProductSku.Id,
                        IsAssigned = licenses.Items
                            .SingleOrDefault(x => x.ProductSku.Name.Equals(sku.ProductSku.Name)) != null ? true : false,
                        Name = sku.ProductSku.Name,
                        SkuPartNumber = sku.ProductSku.SkuPartNumber,
                        TargetType = sku.ProductSku.TargetType,
                        TotalUnits = sku.TotalUnits
                    };

                    values.Add(licenseModel);
                }

                return values;
            }
            finally
            {
                licenseModel = null;
                licenses = null;
                subscribedSkus = null;
            }
        }

        /// <summary>
        /// Modifies the license represented by the instance of <see cref="EditUserModel"/>.
        /// </summary>
        /// <param name="model">An instance of <see cref="EditUserModel"/> that represents the modifications to be made.</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        private async Task ProcessLicenseModifications(EditUserModel model)
        {
            LicenseUpdate licenseUpdate;
            LicenseModel license;
            List<LicenseModel> current;
            List<LicenseAssignment> assignments;
            List<string> removals;

            try
            {
                assignments = new List<LicenseAssignment>();
                current = await this.GetLicenses(model.CustomerId, model.UserId);
                licenseUpdate = new LicenseUpdate();
                removals = new List<string>();

                foreach (LicenseModel item in current)
                {
                    license = model.Licenses.SingleOrDefault(x => x.Id.Equals(item.Id, StringComparison.CurrentCultureIgnoreCase));

                    if (license == null)
                    {
                        continue;
                    }

                    if (!item.IsAssigned && license.IsAssigned)
                    {
                        assignments.Add(new LicenseAssignment() { ExcludedPlans = null, SkuId = license.Id });
                    }
                    else if (item.IsAssigned && !license.IsAssigned)
                    {
                        removals.Add(license.Id);
                    }
                }

                if (assignments.Count > 0)
                {
                    licenseUpdate.LicensesToAssign = assignments;
                }

                if (removals.Count > 0)
                {
                    licenseUpdate.LicensesToRemove = removals;
                }

                if (assignments.Count > 0 || removals.Count > 0)
                {
                    await this.Service.PartnerCenter.Customers.ById(model.CustomerId)
                        .Users.ById(model.UserId).LicenseUpdates.CreateAsync(licenseUpdate);
                }
            }
            finally
            {
                assignments = null;
                current = null;
                license = null;
                licenseUpdate = null;
                removals = null;
            }
        }
    }
}