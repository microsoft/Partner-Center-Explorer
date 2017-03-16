// -----------------------------------------------------------------------
// <copyright file="AuthenticationFilter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Security
{
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Filters;

    /// <summary>
    /// Augments MVC authentication by replacing the principal with a more usable customer portal principal object.
    /// </summary>
    public class AuthenticationFilter : ActionFilterAttribute, IAuthenticationFilter
    {
        /// <summary>
        /// Injects a more friendly claims principal of type <see cref="CustomerPrincipal"/>.
        /// </summary>
        /// <param name="filterContext">The context to use for authentication.</param>
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            filterContext.Principal = new CustomerPrincipal(HttpContext.Current.User as System.Security.Claims.ClaimsPrincipal);
        }

        /// <summary>
        /// Adds an authentication challenge to the current <see cref="ActionResult"/>.
        /// </summary>
        /// <param name="filterContext">The context to use for the authentication challenge.</param>
        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
        }
    }
}