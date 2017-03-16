// -----------------------------------------------------------------------
// <copyright file="ITokenManagement.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Security
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a management interface for retrieving access tokens.
    /// </summary>
    public interface ITokenManagement
    {
        /// <summary>
        /// Gets an access token from the authority using app only authentication.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <param name="resource">Identifier of the target resource that is the recipient of the requested token.</param>
        /// <returns>An instance of <see cref="AuthenticationToken"/> that represented the access token.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="authority"/> is empty or null.
        /// or 
        /// <paramref name="resource"/> is empty or null.
        /// </exception>
        Task<AuthenticationToken> GetAppOnlyTokenAsync(string authority, string resource);

        /// <summary>
        /// Gets an access token from the authority using app only authentication.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <param name="resource">Identifier of the target resource that is the recipient of the requested token.</param>
        /// <param name="scope">Permissions the requested token will need.</param>
        /// <returns>A string that represented the access token.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="authority"/> is empty or null.
        /// or
        /// <paramref name="resource"/> is empty or null.
        /// </exception>
        Task<string> GetAppOnlyTokenAsync(string authority, string resource, string scope);

        /// <summary>
        /// Gets an access token from the authority using app + user authentication.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <param name="resource">Identifier of the target resource that is the recipient of the requested token.</param>
        /// <returns>An instance of <see cref="AuthenticationToken"/> that represented the access token.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="authority"/> is empty or null.
        /// or
        /// <paramref name="resource"/> is empty or null.
        /// </exception>
        AuthenticationToken GetAppPlusUserToken(string authority, string resource);

        /// <summary>
        /// Gets an access token from the authority using app + user authentication.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <param name="resource">Identifier of the target resource that is the recipient of the requested token.</param>
        /// <returns>An instance of <see cref="AuthenticationToken"/> that represented the access token.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="authority"/> is empty or null.
        /// or
        /// <paramref name="resource"/> is empty or null.
        /// </exception>
        Task<AuthenticationToken> GetAppPlusUserTokenAsync(string authority, string resource);

        /// <summary>
        /// Gets an instance of <see cref="IPartnerCredentials"/> used to access the Partner Center API.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <returns>
        /// An instance of <see cref="IPartnerCredentials" /> that represents the access token.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="authority"/> is empty or null.
        /// </exception>
        /// <remarks>
        /// This function will use app only authentication to obtain the credentials.
        /// </remarks>
        IPartnerCredentials GetPartnerCenterAppOnlyCredentials(string authority);

        /// <summary>
        /// Gets an instance of <see cref="IPartnerCredentials"/> used to access the Partner Center API.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <returns>
        /// An instance of <see cref="IPartnerCredentials" /> that represents the access token.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="authority"/> is empty or null.
        /// </exception>
        /// <remarks>
        /// This function will use app only authentication to obtain the credentials.
        /// </remarks>
        Task<IPartnerCredentials> GetPartnerCenterAppOnlyCredentialsAsync(string authority);

        /// <summary>
        /// Gets an instance of <see cref="IPartnerCredentials"/> used to access the Partner Center API.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <returns>
        /// An instance of <see cref="IPartnerCredentials" /> that represents the access token.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="authority"/> is empty or null.
        /// </exception>
        /// <remarks>
        /// This function will use app plus user authentication to obtain the credentials.
        /// </remarks>
        IPartnerCredentials GetPartnerCenterAppPlusUserCredentials(string authority);

        /// <summary>
        /// Gets an instance of <see cref="IPartnerCredentials"/> used to access the Partner Center API.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <returns>
        /// An instance of <see cref="IPartnerCredentials" /> that represents the access token.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="authority"/> is empty or null.
        /// </exception>
        /// <remarks>
        /// This function will use app plus user authentication to obtain the credentials.
        /// </remarks>
        Task<IPartnerCredentials> GetPartnerCenterAppPlusUserCredentialsAsync(string authority);

        /// <summary>
        /// Gets an access token utilizing an authorization code. 
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <param name="code">Authorization code received from the service authorization endpoint.</param>
        /// <param name="resource">Identifier of the target resource that is the recipient of the requested token.</param>
        /// <param name="redirectUri">Redirect URI used for obtain the authorization code.</param>
        /// <returns>An instance of <see cref="AuthenticationToken"/> that represented the access token.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="authority"/> is empty or null.
        /// or
        /// <paramref name="code"/> is empty or null.
        /// or
        /// <paramref name="resource"/> is empty or null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="redirectUri"/> is null.
        /// </exception>
        AuthenticationToken GetTokenByAuthorizationCode(string authority, string code, string resource, Uri redirectUri);

        /// <summary>
        /// Gets an access token utilizing an authorization code. 
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <param name="code">Authorization code received from the service authorization endpoint.</param>
        /// <param name="resource">Identifier of the target resource that is the recipient of the requested token.</param>
        /// <param name="redirectUri">Redirect URI used for obtain the authorization code.</param>
        /// <returns>An instance of <see cref="AuthenticationToken"/> that represented the access token.</returns>
        /// <exception cref="ArgumentException">
        /// authority
        /// or
        /// code
        /// or
        /// resource
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// redirectUri
        /// </exception>
        Task<AuthenticationToken> GetTokenByAuthorizationCodeAsync(string authority, string code, string resource, Uri redirectUri);
    }
}