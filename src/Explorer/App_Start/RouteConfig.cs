// -----------------------------------------------------------------------
// <copyright file="RouteConfig.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer
{
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// Configures routing for the application.
    /// </summary>
    public static class RouteConfig
    {
        /// <summary>
        /// Registers the routes for the application.
        /// </summary>
        /// <param name="routes">A collection of routes.</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "AuditAction",
                url: "Audit/{action}",
                defaults: new { controller = "Audit" });

            routes.MapRoute(
                 name: "CustomerAction",
                 url: "Customers/{action}",
                 defaults: new { controller = "Customers" });

            routes.MapRoute(
                name: "CustomersActions",
                url: "Customers/{customerId}/{action}",
                defaults: new { controller = "Customers" });

            routes.MapRoute(
                name: "Domains",
                url: "Customers/{customerId}/Domains/{domain}/{action}",
                defaults: new { controller = "Domains" });

            routes.MapRoute(
                name: "Invoices",
                url: "Invoices/{invoiceId}/{action}",
                defaults: new { controller = "Invoices", action = "Details" });

            routes.MapRoute(
                name: "InvoiceDetails",
                url: "Invoices/{invoiceId}/{customerName}/{action}",
                defaults: new { controller = "Invoices" });

            routes.MapRoute(
                name: "Subscriptions",
                url: "Customers/{customerId}/Subscriptions/{action}",
                defaults: new { controller = "Subscriptions" });

            routes.MapRoute(
                name: "Subscription",
                url: "Customers/{customerId}/Subscriptions/{subscriptionId}/{action}",
                defaults: new { controller = "Subscriptions", action = "Index" });

            routes.MapRoute(
                name: "SubscriptionHealth",
                url: "Customers/{customerId}/Health/{subscriptionId}/",
                defaults: new { controller = "Health", action = "Index" });

            routes.MapRoute(
                name: "SubscriptionManageResource",
                url: "Customers/{customerId}/Manage/{subscriptionId}/{action}",
                defaults: new { controller = "Manage", action = "Index" });

            routes.MapRoute(
                name: "Usage",
                url: "Customers/{customerId}/Usage/{subscriptionId}/",
                defaults: new { controller = "Usage", action = "ViewUsage" });

            routes.MapRoute(
                name: "UserActions",
                url: "Customers/{customerId}/Users/{action}",
                defaults: new { controller = "Users" });

            routes.MapRoute(
                name: "Users",
                url: "Customers/{customerId}/users/{userId}/{action}",
                defaults: new { controller = "Users" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}