// -----------------------------------------------------------------------
// <copyright file="AuditSearchModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Model that represents audit record log search.
    /// </summary>
    public class AuditSearchModel
    {
        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }
    }
}