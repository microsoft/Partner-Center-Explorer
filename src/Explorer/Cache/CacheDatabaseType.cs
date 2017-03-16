// -----------------------------------------------------------------------
// <copyright file="CacheDatabaseType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Cache
{
    /// <summary>
    /// Defines the different cache databases.
    /// </summary>
    public enum CacheDatabaseType
    {
        /// <summary>
        /// Cache database used to store authentication related entities.
        /// </summary>
        Authentication = 0,

        /// <summary>
        /// Cache database used to store various data structures.
        /// </summary>
        DataStructures = 1
    }
}