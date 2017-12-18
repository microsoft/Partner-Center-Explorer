// -----------------------------------------------------------------------
// <copyright file="ServiceRequestsController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Logic;
    using Models;
    using PartnerCenter.Models;
    using PartnerCenter.Models.ServiceRequests;
    using Security;

    /// <summary>
    /// Handles all request for Service Requests views.
    /// </summary>
    [AuthorizationFilter(Roles = UserRole.Partner)]
    public class ServiceRequestsController : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRequestsController"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        public ServiceRequestsController(IExplorerService service) : base(service)
        {
        }

        /// <summary>
        /// Serves the HTML template for the index page.
        /// </summary>
        /// <returns>
        /// The HTML template for the index page.
        /// </returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Serves the HTML template for the list view.
        /// </summary>
        /// <returns>The HTML template for the list view.</returns>
        public async Task<PartialViewResult> List()
        {
            ServiceRequestsModel serviceRequestsModel = new ServiceRequestsModel()
            {
                ServiceRequests = await GetServiceRequestsAsync().ConfigureAwait(false)
            };

            return PartialView(serviceRequestsModel);
        }

        /// <summary>
        /// Gets a list of service requests.
        /// </summary>
        /// <returns>A list of service requests.</returns>
        private async Task<List<ServiceRequestModel>> GetServiceRequestsAsync()
        {
            ResourceCollection<ServiceRequest> requests;

            try
            {
                requests = await Service.PartnerOperations.GetServiceRequestsAsync().ConfigureAwait(false);

                return requests.Items.Select(r => new ServiceRequestModel()
                {
                    CreatedDate = r.CreatedDate,
                    Id = r.Id,
                    Organization = r.Organization?.Name,
                    PrimaryContact = r.PrimaryContact?.Email,
                    ProductName = r.ProductName,
                    Status = r.Status.ToString(),
                    Title = r.Title
                }).ToList();
            }
            finally
            {
                requests = null;
            }
        }
    }
}