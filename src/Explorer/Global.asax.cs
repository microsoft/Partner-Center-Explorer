// -----------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer
{
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using Logic;
    using Unity;

    /// <summary>
    /// Defines the methods and properties that are common to application objects.
    /// </summary>
    /// <seealso cref="System.Web.HttpApplication" />
    public class MvcApplication : HttpApplication
    {
        /// <summary>
        /// Called when the application starts.
        /// </summary>
        protected void Application_Start()
        {
            IExplorerService service;

            try
            {
                AreaRegistration.RegisterAllAreas();

                service = UnityConfig.Container.Resolve<IExplorerService>();

                Task.Run(service.InitializeAsync).Wait();

                ApplicationInsights.Extensibility.TelemetryConfiguration.Active.InstrumentationKey =
                    service.Configuration.InstrumentationKey;

                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);
            }
            finally
            {
                service = null; 
            }
        }
    }
}