// -----------------------------------------------------------------------
// <copyright file="ServiceRequestModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using System;

    /// <summary>
    /// Represents a service request.
    /// </summary>
    public class ServiceRequestModel
    {
        /// <summary>
        /// Gets or sets the created date for the service request.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the service request.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the organization for the service request.
        /// </summary>
        public string Organization { get; set; }

        /// <summary>
        /// Gets or sets the primary contact for the service request.
        /// </summary>
        public string PrimaryContact { get; set; }

        /// <summary>
        /// Gets or sets the product name for the service request.
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets the status of the service request.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the title of the service request.
        /// </summary>
        public string Title { get; set; }
    }
}