// -----------------------------------------------------------------------
// <copyright file="AuditController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Models;
    using Providers;
    using Security;

    /// <summary>
    /// Controller for Audit views.
    /// </summary>
    /// <seealso cref="System.Web.Mvc.Controller" />
    [AuthorizationFilter(Roles = UserRoles.Partner)]
    public class AuditController : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuditController"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        public AuditController(IExplorerProvider provider) : base(provider)
        {
        }

        /// <summary>
        /// Get the audit records for the specified date range using the Partner Center API.
        /// </summary>
        /// <param name="endDate">The end date of the audit record logs.</param>
        /// <param name="startDate">The start date of the audit record logs.</param>
        /// <returns>A partial view containing the requested audit record logs.</returns>
        public async Task<PartialViewResult> GetRecords(DateTime endDate, DateTime startDate)
        {
            AuditRecordsModel auditRecordsModel = new AuditRecordsModel()
            {
                Records = await Provider.PartnerOperations.GetAuditRecordsAsync(startDate, endDate).ConfigureAwait(false)
            };

            return PartialView("Records", auditRecordsModel);
        }

        /// <summary>
        /// Handles the request for the Search view.
        /// </summary>
        /// <returns>Returns an empty view.</returns>
        public ActionResult Search()
        {
            return View();
        }
    }
}