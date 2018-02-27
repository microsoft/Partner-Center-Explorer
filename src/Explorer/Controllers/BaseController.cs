// -----------------------------------------------------------------------
// <copyright file="BaseController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Controllers
{
    using System.Web.Mvc;
    using Providers;

    /// <summary>
    /// Base controller for controllers.
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        /// Provides access to the core application services.
        /// </summary>
        private readonly IExplorerProvider provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseController"/> class.
        /// </summary>
        protected BaseController(IExplorerProvider provider)
        {
            this.provider = provider;
        }

        /// <summary>
        /// Provides access to the core application services.
        /// </summary>
        protected IExplorerProvider Provider => provider;
    }
}