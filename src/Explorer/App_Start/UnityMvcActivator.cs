// -----------------------------------------------------------------------
// <copyright file="UnityMvcActivator.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Microsoft.Store.PartnerCenter.Explorer.UnityMvcActivator), nameof(Microsoft.Store.PartnerCenter.Explorer.UnityMvcActivator.Start))]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(Microsoft.Store.PartnerCenter.Explorer.UnityMvcActivator), nameof(Microsoft.Store.PartnerCenter.Explorer.UnityMvcActivator.Shutdown))]

namespace Microsoft.Store.PartnerCenter.Explorer
{
    using System.Linq;
    using System.Web.Mvc;
    using Unity.AspNet.Mvc;

    /// <summary>
    /// Provides the bootstrapping for integrating Unity with ASP.NET MVC.
    /// </summary>
    public static class UnityMvcActivator
    {
        /// <summary>
        /// Integrates Unity when the application starts.
        /// </summary>
        public static void Start()
        {
            FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().First());
            FilterProviders.Providers.Add(new UnityFilterAttributeFilterProvider(UnityConfig.Container));

            DependencyResolver.SetResolver(new UnityDependencyResolver(UnityConfig.Container));
        }

        /// <summary>
        /// Disposes the Unity container when the application is shut down.
        /// </summary>
        public static void Shutdown()
        {
            UnityConfig.Container.Dispose();
        }
    }
}