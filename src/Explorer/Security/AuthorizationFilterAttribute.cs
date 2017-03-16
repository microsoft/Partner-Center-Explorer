// -----------------------------------------------------------------------
// <copyright file="AuthorizationFilterAttribute.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Security
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Logic;

    /// <summary>
    /// Authorization filter attribute used to verify authenticated user has the specified claim and value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AuthorizationFilterAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Gets or sets the required roles.
        /// </summary>
        public new UserRole Roles { get; set; }

        /// <summary>
        /// Verifies the authenticated user has the appropriate privileges.
        /// </summary>
        /// <param name="httpContext">The HTTP context, which encapsulates all HTTP-specific information about an individual HTTP request.</param>
        /// <returns><c>true</c> if the user is authorized; otherwise <c>false</c>.</returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            CustomerPrincipal principal;

            try
            {
                principal = httpContext.User as CustomerPrincipal;

                foreach (string role in this.GetRoles(this.Roles))
                {
                    if (principal.HasClaim(System.Security.Claims.ClaimTypes.Role, role))
                    {
                        return true;
                    }
                }

                return false;
            }
            finally
            {
                principal = null;
            }
        }

        /// <summary>
        /// Processes HTTP requests that fail authorization.
        /// </summary>
        /// <param name="filterContext">Encapsulates the information for using <see cref="System.Web.Mvc.AuthorizeAttribute"/>.</param>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new
                        {
                            controller = "Error",
                            action = "ShowError",
                            errorMessage = "You do not have sufficient priviliges to view this page."
                        }));
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }

        /// <summary>
        /// Gets a list of roles that required to perform the operation.
        /// </summary>
        /// <param name="requiredRole">User role required to perform the operation.</param>
        /// <returns>A list of roles that required to perform the operation.</returns>
        private List<string> GetRoles(UserRole requiredRole)
        {
            List<string> required = new List<string>();

            if (requiredRole.HasFlag(UserRole.AdminAgents))
            {
                required.Add(UserRole.AdminAgents.GetDescription());
            }

            if (requiredRole.HasFlag(UserRole.BillingAdmin))
            {
                required.Add(UserRole.BillingAdmin.GetDescription());
            }

            if (requiredRole.HasFlag(UserRole.GlobalAdmin))
            {
                required.Add(UserRole.GlobalAdmin.GetDescription());
            }

            if (requiredRole.HasFlag(UserRole.HelpdeskAgent))
            {
                required.Add(UserRole.HelpdeskAgent.GetDescription());
            }

            if (requiredRole.HasFlag(UserRole.User))
            {
                required.Add(UserRole.User.GetDescription());
            }

            if (requiredRole.HasFlag(UserRole.UserAdministrator))
            {
                required.Add(UserRole.UserAdministrator.GetDescription());
            }

            return required;
        }
    }
}