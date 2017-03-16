// -----------------------------------------------------------------------
// <copyright file="HomeController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Controllers
{
    using System.Web.Mvc;
    using Logic;
    using Security;

    /// <summary>
    /// Controller for all Home views.
    /// </summary>
    [AuthorizationFilter(Roles = UserRole.Partner)]
    public class HomeController : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        public HomeController(IExplorerService service) : base(service)
        {
        }

        /// <summary>
        /// Handles the request for the index view.
        /// </summary>
        /// <returns>The HTML template for the index page.</returns>
        public ActionResult Index()
        {
            return this.View();
        }

        /// <summary>
        /// Handles the request for the error view.
        /// </summary>
        /// <returns>The HTML template for the error page.</returns>
        public ActionResult Error()
        {
            return this.View();
        }
    }
}