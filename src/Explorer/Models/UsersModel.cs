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
        /// Initializes a new instance of the <see cref="UsersModel" /> class.
        /// </summary>
        public UsersModel()
        {
            Users = new List<UserModel>();
        }

        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets a collection of users that belong to a specific customer.
        /// </summary>
        public List<UserModel> Users { get; }
    }
}