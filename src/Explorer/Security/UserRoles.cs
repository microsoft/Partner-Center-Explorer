// -----------------------------------------------------------------------
// <copyright file="UserRole.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Security
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Defines the various user roles.
    /// </summary>
    [Flags]
    public enum UserRoles
    {
        /// <summary>
        /// Partner Center role that provides complete access to Partner Center.
        /// </summary>
        [Description("AdminAgents")]
        AdminAgents = 0,

        /// <summary>
        /// Role that can make purchases, manages subscriptions, manages support tickets, and monitors service health.
        /// </summary>
        BillingAdmin = 1,

        /// <summary>
        /// Has access to all administrative features at the tenant level. 
        /// </summary>
        [Description("Company Administrator")]
        GlobalAdmin = 2,

        /// <summary>
        /// Partner Center role that provides the ability to support customers.
        /// </summary>
        HelpdeskAgent = 4,

        /// <summary>
        /// Combination of the available partner roles.
        /// </summary>
        Partner = AdminAgents | HelpdeskAgent,

        /// <summary>
        /// Has access to no administrative features.
        /// </summary>
        User = 8,

        /// <summary>
        /// Roles that manage support tickets and users at the tenant level.
        /// </summary>
        UserAdministrator = 16
    }
}