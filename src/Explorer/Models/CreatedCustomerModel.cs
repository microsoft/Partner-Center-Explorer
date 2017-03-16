// -----------------------------------------------------------------------
// <copyright file="CreatedCustomerModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    /// <summary>
    /// Model that represents the newly created customer.
    /// </summary>
    public class CreatedCustomerModel
    {
        /// <summary>
        /// Gets or sets the domain for the user.
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the password configured the initial user.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the username configured for the initial user.
        /// </summary>
        public string Username { get; set; }
    }
}