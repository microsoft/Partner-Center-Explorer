// -----------------------------------------------------------------------
// <copyright file="UnityConfig.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer
{
    using System;
    using Logic;
    using Practices.Unity;

    /// <summary>
    /// Provides configurations for Unity.
    /// </summary>
    public static class UnityConfig
    {
        /// <summary>
        /// The Unity container to be utilized for dependency injection.
        /// </summary>
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            UnityContainer container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        /// <returns>
        /// The configured Unity container.</returns>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<IExplorerService, ExplorerService>();
        }
    }
}