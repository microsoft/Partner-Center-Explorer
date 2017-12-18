// -----------------------------------------------------------------------
// <copyright file="Communication.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic
{
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides the ability to perform HTTP operations.
    /// </summary>
    public class Communication : ICommunication
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Communication"/> class.
        /// </summary>
        /// <param name="service">Provides access to the core services.</param>
        public Communication(IExplorerService service)
        {
        }

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
        public async Task<T> GetAsync<T>(string requestUri, string token)
        {
            HttpResponseMessage response;

            requestUri.AssertNotEmpty(nameof(requestUri));
            token.AssertNotEmpty(nameof(token));

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    response = await client.GetAsync(requestUri).ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        throw new CommunicationException(result, response.StatusCode);
                    }

                    return await response.Content.ReadAsAsync<T>().ConfigureAwait(false);
                }
            }
            finally
            {
                response = null;
            }
        }
    }
}