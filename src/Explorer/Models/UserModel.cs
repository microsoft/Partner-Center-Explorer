// -----------------------------------------------------------------------
// <copyright file="UserModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using System;

    /// <summary>
    /// Model that represent a user.
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the last directory synchronization time.
        /// </summary>
        public DateTime? LastDirectorySyncTime { get; set; }

        /// <summary>
        /// Gets or sets the usage location.
        /// </summary>
        public string UsageLocation { get; set; }

        /// <summary>
        /// Gets or sets the user principal name.
        /// </summary>
        public string UserPrincipalName { get; set; }
    }
}