// -----------------------------------------------------------------------
// <copyright file="TelemetryHandleErrorAttribute.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic
{
    using System;
    using System.Web.Mvc;
    using Providers;
    using Unity;

    /// <summary>
    /// Represents custom handle error attribute that logs the exception to the configured telemetry provider.
    /// </summary>
    /// <seealso cref="HandleErrorAttribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class TelemetryHandleErrorAttribute : HandleErrorAttribute
    {
        /// <summary>
        /// Called when an exception occurs.
        /// </summary>
        /// <param name="filterContext">The action-filter context.</param>
        public override void OnException(ExceptionContext filterContext)
        {
            IExplorerProvider provider;

            try
            {
                provider = UnityConfig.Container.Resolve<IExplorerProvider>();

                if (filterContext?.HttpContext != null && filterContext.Exception != null)
                {
                    if (filterContext.HttpContext.IsCustomErrorEnabled)
                    {
                        provider.Telemetry.TrackException(filterContext.Exception);
                    }
                }

                base.OnException(filterContext);
            }
            finally
            {
                provider = null;
            }
        }
    }
}