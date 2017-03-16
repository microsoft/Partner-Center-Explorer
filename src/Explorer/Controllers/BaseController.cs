// -----------------------------------------------------------------------
// <copyright file="BaseController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Controllers
{
    using System.Web.Mvc;
    using Logic;

    /// <summary>
    /// Base controller for controllers.
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        /// Provides access to the core application services.
        /// </summary>
        private readonly IExplorerService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseController"/> class.
        /// </summary>
        /// <param name="service">Provides access to the core application services.</param>
        protected BaseController(IExplorerService service)
        {
            service.AssertNotNull(nameof(service));

            this.service = service;
        }

        /// <summary>
        /// Provides access to the core application services.
        /// </summary>
        protected IExplorerService Service => this.service;
    }
}