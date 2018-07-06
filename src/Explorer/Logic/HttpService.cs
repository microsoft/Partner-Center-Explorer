// -----------------------------------------------------------------------
// <copyright file="HttpService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Rest;

    /// <summary>
    /// Provides the ability to perform HTTP operations.
    /// </summary>
    public class HttpService : IHttpService
    {
        /// <summary>
        /// Static instance of the <see cref="HttpClient" /> class.
        /// </summary>
        private static HttpClient client = new HttpClient(
            new RetryDelegatingHandler
            {
                InnerHandler = new HttpClientHandler()
            });

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
        public async Task<T> GetAsync<T>(Uri requestUri, string token)
        {
            HttpResponseMessage response;

            requestUri.AssertNotNull(nameof(requestUri));
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