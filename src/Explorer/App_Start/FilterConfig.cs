// -----------------------------------------------------------------------
// <copyright file="FilterConfig.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer
{
    using System.Web.Mvc;
    using Logic; 
    using Security;

    /// <summary>
    /// Configures the application filters.
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// Registers the global filters.
        /// </summary>
        /// <param name="filters">The global filter collection.</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new AuthenticationFilter());
            filters.Add(new TelemetryHandleErrorAttribute());
        }
    }
}