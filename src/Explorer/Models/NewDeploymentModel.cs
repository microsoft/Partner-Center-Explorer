// -----------------------------------------------------------------------
// <copyright file="NewDeploymentModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Model for new Azure Resource Manager (ARM) template deployments.
    /// </summary>
    public class NewDeploymentModel
    {
        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the URI where the parameters JSON is located.
        /// </summary>
        [Display(Name = "Parameters Link")]
        [Required]
        public Uri ParametersUri { get; set; }

        /// <summary>
        /// Gets or sets the name of the resource group.
        /// </summary>
        public string ResourceGroupName { get; set; }

        /// <summary>
        /// Gets or sets the subscription identifier.
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the URI where the template JSON is located.
        /// </summary>
        [Display(Name = "Template Link")]
        [Required]
        public Uri TemplateUri { get; set; }
    }
}