// -----------------------------------------------------------------------
// <copyright file="ServiceRequestsModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Model for service requests from Partner Center.
    /// </summary>
    public class ServiceRequestsModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRequestsModel" /> class.
        /// </summary>
        public ServiceRequestsModel()
        {
            ServiceRequests = new List<ServiceRequestModel>();
        }

        /// <summary>
        /// Gets the service requests.
        /// </summary>
        public List<ServiceRequestModel> ServiceRequests { get; }
    }
}