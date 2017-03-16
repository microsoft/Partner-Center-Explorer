// -----------------------------------------------------------------------
// <copyright file="DeploymentModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using System;

    /// <summary>
    /// Model for new deployments in Azure using Azure Resource Manager (ARM) templates.
    /// </summary>
    public class DeploymentModel
    {
        /// <summary>
        /// Gets or sets the identifier for the deployment.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name for the deployment
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the state of the provisioning.
        /// </summary>
        public string ProvisioningState { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}