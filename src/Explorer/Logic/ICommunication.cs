// -----------------------------------------------------------------------
// <copyright file="ICommunication.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic
{
    using System.Threading.Tasks;

    /// <summary>
    /// Represent the ability to perform HTTP operations.
    /// </summary>
    public interface ICommunication
    {
        /// <summary>
        /// Sends a GET request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="requestUri">The Uri where the request should be sent.</param>
        /// <param name="token">The access token value used to authorize the request.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="requestUri"/> is empty or null.
        /// or
        /// <paramref name="token"/> is empty or null.
        /// </exception>
        /// <exception cref="CommunicationException"></exception>
        Task<T> GetAsync<T>(string requestUri, string token);
    }
}