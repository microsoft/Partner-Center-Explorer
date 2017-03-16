// -----------------------------------------------------------------------
// <copyright file="CustomersModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Model for listing existing customers.
    /// </summary>
    public class CustomersModel
    {
        /// <summary>
        /// Gets or sets a collection of customer that belong to the configured partner.
        /// </summary>
        public List<CustomerModel> Customers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the configured tenant is the integration sandbox.
        /// </summary>
        public bool IsSandboxEnvironment { get; set; }
    }
}