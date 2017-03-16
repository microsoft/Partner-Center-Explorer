// -----------------------------------------------------------------------
// <copyright file="UsersModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Model for listing users.
    /// </summary>
    public class UsersModel
    {
        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets a collection of users that belong to a specific customer.
        /// </summary>
        public List<UserModel> Users { get; set; }
    }
}