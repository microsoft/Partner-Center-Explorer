// -----------------------------------------------------------------------
// <copyright file="CustomersController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Logic;
    using Models;
    using PartnerCenter.Models;
    using PartnerCenter.Models.Customers;
    using Providers;
    using Security;

    /// <summary>
    /// Controller for all Customers views.
    /// </summary>
    [AuthorizationFilter(Roles = UserRole.Partner)]
    public class CustomersController : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomersController"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        public CustomersController(IExplorerProvider provider) : base(provider)
        {
        }

        /// <summary>
        /// Deletes the specified customer.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>Returns the NoContent HTTP status code.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// </exception>
        [HttpDelete]
        public async Task<HttpResponseMessage> Delete(string customerId)
        {
            customerId.AssertNotEmpty(nameof(customerId));

            if (Provider.Configuration.IsIntegrationSandbox)
            {
                await Provider.PartnerOperations.DeleteCustomerAsync(customerId).ConfigureAwait(false);
            }

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Serves the HTML template for the create view.
        /// </summary>
        /// <returns>The HTML template for the create view.</returns>
        [HttpGet]
        public PartialViewResult Create()
        {
            return PartialView();
        }

        /// <summary>
        /// Create the customer represented by the instance of <see cref="NewCustomerModel"/>.
        /// </summary>
        /// <param name="newCustomerModel">The new customer model.</param>
        /// <returns>A partial view containing result of the customer creation.</returns>
        [HttpPost]
        public async Task<PartialViewResult> Create(NewCustomerModel newCustomerModel)
        {
            Customer entity;
            CreatedCustomerModel createdCustomerModel;

            try
            {
                entity = new Customer()
                {
                    BillingProfile = new CustomerBillingProfile()
                    {
                        CompanyName = newCustomerModel.Name,
                        Culture = "en-US",
                        DefaultAddress = new Address()
                        {
                            AddressLine1 = newCustomerModel.AddressLine1,
                            AddressLine2 = newCustomerModel.AddressLine2,
                            City = newCustomerModel.City,
                            Country = "US",
                            FirstName = newCustomerModel.FirstName,
                            LastName = newCustomerModel.LastName,
                            PhoneNumber = newCustomerModel.PhoneNumber,
                            PostalCode = newCustomerModel.ZipCode,
                            State = newCustomerModel.State
                        },
                        Email = newCustomerModel.EmailAddress,
                        FirstName = newCustomerModel.FirstName,
                        Language = "en",
                        LastName = newCustomerModel.LastName
                    },
                    CompanyProfile = new CustomerCompanyProfile()
                    {
                        CompanyName = newCustomerModel.Name,
                        Domain = $"{newCustomerModel.PrimaryDomain}.onmicrosoft.com"
                    }
                };

                entity = await Provider.PartnerOperations.CreateCustomerAsync(entity).ConfigureAwait(false);

                createdCustomerModel = new CreatedCustomerModel()
                {
                    Domain = $"{newCustomerModel.PrimaryDomain}.onmicrosoft.com",
                    Password = entity.UserCredentials.Password,
                    Username = entity.UserCredentials.UserName
                };

                return PartialView("CreatedSuccessfully", createdCustomerModel);
            }
            finally
            {
                entity = null;
            }
        }

        /// <summary>
        /// Serves the HTML template for the list page.
        /// </summary>
        /// <returns>The HTML template for the list page.</returns>
        public async Task<ActionResult> List()
        {
            CustomersModel customersModel = new CustomersModel()
            {
                Customers = await GetCustomerModelsAsync().ConfigureAwait(false),
                IsSandboxEnvironment = Provider.Configuration.IsIntegrationSandbox
            };

            return PartialView(customersModel);
        }

        /// <summary>
        /// Serves the HTML template for the index page. 
        /// </summary>
        /// <returns>The HTML template for the index page.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Handles the request to view the customer associated with the specified customer identifier.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>A view to display the customer details.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// </exception>
        public async Task<ActionResult> Show(string customerId)
        {
            Customer customer;
            CustomerModel customerModel;

            customerId.AssertNotEmpty(nameof(customerId));

            try
            {
                customer = await Provider.PartnerOperations.GetCustomerAsync(customerId).ConfigureAwait(false);

                customerModel = new CustomerModel()
                {
                    BillingProfile = customer.BillingProfile,
                    CompanyName = customer.CompanyProfile.CompanyName,
                    CompanyProfile = customer.CompanyProfile,
                    DomainName = customer.CompanyProfile.Domain,
                    CustomerId = customer.CompanyProfile.TenantId
                };

                return View(customerModel);
            }
            finally
            {
                customer = null;
                customerModel = null;
            }
        }

        /// <summary>
        /// Gets a list of customers.
        /// </summary>
        /// <returns>A list of <see cref="CustomerModel"/>s that represents the customers.</returns>
        private async Task<List<CustomerModel>> GetCustomerModelsAsync()
        {
            List<Customer> customers;

            try
            {
                customers = await Provider.PartnerOperations.GetCustomersAsync().ConfigureAwait(false);

                return customers.Select(item => new CustomerModel
                {
                    BillingProfile = new CustomerBillingProfile
                    {
                        CompanyName = item.CompanyProfile.CompanyName,
                    },
                    CompanyName = item.CompanyProfile.CompanyName,
                    CustomerId = item.Id,
                    CompanyProfile = new CustomerCompanyProfile
                    {
                        CompanyName = item.CompanyProfile.CompanyName,
                        Domain = item.CompanyProfile.Domain,
                        TenantId = item.CompanyProfile.TenantId
                    },
                    DomainName = item.CompanyProfile.Domain,
                    RelationshipToPartner = item.RelationshipToPartner.ToString()
                }).ToList();
            }
            finally
            {
                customers = null;
            }
        }
    }
}