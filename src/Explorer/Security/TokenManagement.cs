// -----------------------------------------------------------------------
// <copyright file="TokenManagement.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Security
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Cache;
    using Extensions;
    using IdentityModel.Clients.ActiveDirectory;
    using Logic;
    using Models;

    /// <summary>
    /// Provides the ability to manage access tokens.
    /// </summary>
    public class TokenManagement : ITokenManagement
    {
        /// <summary>
        /// Type of the assertion representing the user when performing app + user authentication.
        /// </summary>
        private const string AssertionType = "urn:ietf:params:oauth:grant-type:jwt-bearer";

        /// <summary>
        /// Key utilized to retrieve and store Partner Center access tokens. 
        /// </summary>
        private const string PartnerCenterCacheKey = "Resource::PartnerCenter::AppOnly";

        /// <summary>
        /// Provides access to the application core services.
        /// </summary>
        private readonly IExplorerService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenManagement"/> class.
        /// </summary>
        /// <param name="service">Provides access to the application core services.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is null.
        /// </exception>
        public TokenManagement(IExplorerService service)
        {
            service.AssertNotNull(nameof(service));
            this.service = service;
        }

        /// <summary>
        /// Gets the token for the current authenticated user.
        /// </summary>
        private static string UserAssertionToken
        {
            get
            {
                System.IdentityModel.Tokens.BootstrapContext bootstrapContext;

                try
                {
                    bootstrapContext = ClaimsPrincipal.Current.Identities.First().BootstrapContext as System.IdentityModel.Tokens.BootstrapContext;

                    return bootstrapContext?.Token;
                }
                finally
                {
                    bootstrapContext = null;
                }
            }
        }

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
        public async Task<AuthenticationToken> GetAppOnlyTokenAsync(string authority, string resource)
        {
            AuthenticationContext authContext;
            AuthenticationResult authResult;
            DistributedTokenCache tokenCache;

            authority.AssertNotEmpty(nameof(authority));
            resource.AssertNotEmpty(nameof(resource));

            try
            {
                if (this.service.Cache.IsEnabled)
                {
                    tokenCache = new DistributedTokenCache(this.service, resource, $"AppOnly::{authority}::{resource}");
                    authContext = new AuthenticationContext(authority, tokenCache);
                }
                else
                {
                    authContext = new AuthenticationContext(authority);
                }

                authResult = await authContext.AcquireTokenAsync(
                    resource,
                    new ClientCredential(
                        this.service.Configuration.ApplicationId,
                        this.service.Configuration.ApplicationSecret));

                return new AuthenticationToken(authResult.AccessToken, authResult.ExpiresOn);
            }
            finally
            {
                authContext = null;
                authResult = null;
                tokenCache = null;
            }
        }

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
        public async Task<string> GetAppOnlyTokenAsync(string authority, string resource, string scope)
        {
            AuthenticationContext authContext;
            AuthenticationResult authResult;
            X509Certificate2 certificate;

            authority.AssertNotEmpty(nameof(authority));
            resource.AssertNotEmpty(nameof(resource));

            try
            {
                authContext = new AuthenticationContext(authority);

                certificate = FindCertificateByThumbprint(this.service.Configuration.VaultApplicationCertThumbprint);

                if (certificate == null)
                {
                    throw new ApplicationException("Unable to locate the certificate.");
                }

                authResult = await authContext.AcquireTokenAsync(
                    resource,
                    new ClientAssertionCertificate(
                        this.service.Configuration.VaultApplicationId,
                        certificate));

                return authResult.AccessToken;
            }
            finally
            {
                authContext = null;
                authResult = null;
                certificate = null;
            }
        }

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
        public AuthenticationToken GetAppPlusUserToken(string authority, string resource)
        {
            authority.AssertNotEmpty(nameof(authority));
            resource.AssertNotEmpty(nameof(resource));

            return SynchronousExecute(() => this.GetAppPlusUserTokenAsync(authority, resource));
        }

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
        public async Task<AuthenticationToken> GetAppPlusUserTokenAsync(string authority, string resource)
        {
            AuthenticationContext authContext;
            AuthenticationResult authResult;
            DistributedTokenCache tokenCache;

            authority.AssertNotEmpty(nameof(authority));
            resource.AssertNotEmpty(nameof(resource));

            try
            {
                if (this.service.Cache.IsEnabled)
                {
                    tokenCache = new DistributedTokenCache(this.service, resource);
                    authContext = new AuthenticationContext(authority, tokenCache);
                }
                else
                {
                    authContext = new AuthenticationContext(authority);
                }

                try
                {
                    authResult = await authContext.AcquireTokenAsync(
                        resource,
                        new ClientCredential(
                            this.service.Configuration.ApplicationId,
                            this.service.Configuration.ApplicationSecret),
                        new UserAssertion(UserAssertionToken, AssertionType));
                }
                catch (AdalServiceException ex)
                {
                    if (ex.ErrorCode.Equals("AADSTS70002", StringComparison.CurrentCultureIgnoreCase))
                    {
                        await this.service.Cache.DeleteAsync(CacheDatabaseType.Authentication);

                        authResult = await authContext.AcquireTokenAsync(
                            resource,
                            new ClientCredential(
                                this.service.Configuration.ApplicationId,
                                this.service.Configuration.ApplicationSecret),
                            new UserAssertion(UserAssertionToken, AssertionType));
                    }
                    else
                    {
                        throw; 
                    }
                }

                return new AuthenticationToken(authResult.AccessToken, authResult.ExpiresOn);
            }
            finally
            {
                authContext = null;
                authResult = null;
                tokenCache = null;
            }
        }

        /// <summary>
        /// Gets an instance of <see cref="IPartnerCredentials"/> used to access the Partner Center Managed API.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <returns>
        /// An instance of <see cref="IPartnerCredentials" /> that represents the access token.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="authority"/> is empty or null.
        /// </exception>
        /// <remarks>This function will use app only authentication to obtain the credentials.</remarks>
        public IPartnerCredentials GetPartnerCenterAppOnlyCredentials(string authority)
        {
            authority.AssertNotEmpty(nameof(authority));

            return SynchronousExecute(() => this.GetPartnerCenterAppOnlyCredentialsAsync(authority));
        }

        /// <summary>
        /// Gets an instance of <see cref="IPartnerCredentials"/> used to access the Partner Center Managed API.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <returns>
        /// An instance of <see cref="IPartnerCredentials" /> that represents the access token.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="authority"/> is empty or null.
        /// </exception>
        /// <remarks>This function will use app only authentication to obtain the credentials.</remarks>
        public async Task<IPartnerCredentials> GetPartnerCenterAppOnlyCredentialsAsync(string authority)
        {
            authority.AssertNotEmpty(nameof(authority));

            // Attempt to obtain the Partner Center token from the cache.
            IPartnerCredentials credentials =
                 await this.service.Cache.FetchAsync<PartnerCenterTokenModel>(
                     CacheDatabaseType.Authentication, PartnerCenterCacheKey);

            if (credentials != null && !credentials.IsExpired())
            {
                return credentials;
            }

            // The access token has expired, so a new one must be requested.
            credentials = await PartnerCredentials.Instance.GenerateByApplicationCredentialsAsync(
                this.service.Configuration.PartnerCenterApplicationId,
                this.service.Configuration.PartnerCenterApplicationSecret,
                this.service.Configuration.PartnerCenterApplicationTenantId);

            await this.service.Cache.StoreAsync(
                CacheDatabaseType.Authentication, PartnerCenterCacheKey, credentials);

            return credentials;
        }

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
        public IPartnerCredentials GetPartnerCenterAppPlusUserCredentials(string authority)
        {
            authority.AssertNotEmpty(nameof(authority));

            return SynchronousExecute(() => this.GetPartnerCenterAppPlusUserCredentialsAsync(authority));
        }

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
        public async Task<IPartnerCredentials> GetPartnerCenterAppPlusUserCredentialsAsync(string authority)
        {
            authority.AssertNotEmpty(nameof(authority));

            string key = $"Resource::PartnerCenter::{ClaimsPrincipal.Current.Identities.First().FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value}";

            IPartnerCredentials credentials =
                 await this.service.Cache.FetchAsync<PartnerCenterTokenModel>(
                     CacheDatabaseType.Authentication, key);

            if (credentials != null && !credentials.IsExpired())
            {
                return credentials;
            }

            AuthenticationToken token = await this.GetAppPlusUserTokenAsync(
                 authority,
                 this.service.Configuration.PartnerCenterEndpoint);

            credentials = await PartnerCredentials.Instance.GenerateByUserCredentialsAsync(
                this.service.Configuration.PartnerCenterApplicationId, token);

            await this.service.Cache.StoreAsync(
               CacheDatabaseType.Authentication, key, credentials);

            return credentials;
        }

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
        public AuthenticationToken GetTokenByAuthorizationCode(string authority, string code, string resource, Uri redirectUri)
        {
            authority.AssertNotEmpty(nameof(authority));
            code.AssertNotEmpty(nameof(code));
            redirectUri.AssertNotNull(nameof(redirectUri));
            resource.AssertNotEmpty(nameof(resource));

            return SynchronousExecute(() => this.GetTokenByAuthorizationCodeAsync(authority, code, resource, redirectUri));
        }

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
        public async Task<AuthenticationToken> GetTokenByAuthorizationCodeAsync(string authority, string code, string resource, Uri redirectUri)
        {
            AuthenticationContext authContext;
            AuthenticationResult authResult;
            DistributedTokenCache tokenCache;

            authority.AssertNotEmpty(nameof(authority));
            code.AssertNotEmpty(nameof(code));
            redirectUri.AssertNotNull(nameof(redirectUri));
            resource.AssertNotEmpty(nameof(resource));

            try
            {
                if (this.service.Cache.IsEnabled)
                {
                    tokenCache = new DistributedTokenCache(this.service, resource);
                    authContext = new AuthenticationContext(authority, tokenCache);
                }
                else
                {
                    authContext = new AuthenticationContext(authority);
                }

                authResult = await authContext.AcquireTokenByAuthorizationCodeAsync(
                    code,
                    redirectUri,
                    new ClientCredential(
                        this.service.Configuration.ApplicationId,
                        this.service.Configuration.ApplicationSecret),
                    resource);

                return new AuthenticationToken(authResult.AccessToken, authResult.ExpiresOn);
            }
            finally
            {
                authContext = null;
                tokenCache = null;
            }
        }

        /// <summary>
        /// Locates a certificate by thumbprint.
        /// </summary>
        /// <param name="thumbprint">Thumbprint of the certificate to be located.</param>
        /// <returns>An instance of <see cref="X509Certificate2"/> that represents the certificate.</returns>
        private static X509Certificate2 FindCertificateByThumbprint(string thumbprint)
        {
            X509Store store = null;
            X509Certificate2Collection col;

            thumbprint.AssertNotNull(nameof(thumbprint));

            try
            {
                store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly);

                col = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
                return col.Count == 0 ? null : col[0];
            }
            finally
            {
                col = null;
                store?.Close();
                store = null;
            }
        }

        /// <summary>
        /// Executes an asynchronous method synchronously
        /// </summary>
        /// <typeparam name="T">The type to be returned.</typeparam>
        /// <param name="operation">The asynchronous operation to be executed.</param>
        /// <returns>The result from the operation.</returns>
        private static T SynchronousExecute<T>(Func<Task<T>> operation)
        {
            try
            {
                return Task.Run(async () => await operation()).Result;
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }
        }
    }
}