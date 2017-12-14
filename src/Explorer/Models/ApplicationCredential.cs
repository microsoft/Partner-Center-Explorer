// -----------------------------------------------------------------------
// <copyright file="ApplicationCredential.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Models
{
    using System.Security;

    /// <summary>
    /// Represents credentials to be used to acquire an access token.
    /// </summary>
    public sealed class ApplicationCredential
    {
        /// <summary>
        /// Identifier of the client requesting the token.
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        /// Secret of the client requesting the token.
        /// </summary>
        public SecureString ApplicationSecret { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether or not distributed token cache 
        /// should be utilized for token acquistion. 
        /// </summary>
        public bool UseCache { get; set; }
    }
}