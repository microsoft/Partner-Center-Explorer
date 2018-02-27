// -----------------------------------------------------------------------
// <copyright file="Startup.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer
{
    using global::Owin;

    /// <summary>
    /// Manage the application startup.
    /// </summary>
    public partial class Startup
    {
        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}