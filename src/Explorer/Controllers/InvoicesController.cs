// -----------------------------------------------------------------------
// <copyright file="InvoicesController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Caching;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using CsvHelper;
    using Logic;
    using Models;
    using PartnerCenter.Models;
    using PartnerCenter.Models.Invoices;
    using Security;

    /// <summary>
    /// Control invoice related operations.
    /// </summary>
    [AuthorizationFilter(Roles = UserRole.Partner)]
    public class InvoicesController : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoicesController"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        public InvoicesController(IExplorerService service) : base(service)
        {
        }

        /// <summary>
        /// Handles the partial view request for Azure details.
        /// </summary>
        /// <param name="customerName">Name of the customer.</param>
        /// <param name="invoiceId">The invoice identifier.</param>
        /// <returns>A partial view containing the InvoiceDetailsModel model.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="customerName"/> is empty or null.
        /// or 
        /// <paramref name="invoiceId"/> is empty or null.
        /// </exception>
        public async Task<PartialViewResult> AzureDetails(string customerName, string invoiceId)
        {
            customerName.AssertNotEmpty(nameof(customerName));
            invoiceId.AssertNotEmpty(nameof(invoiceId));

            InvoiceDetailsModel invoiceDetailsModel = new InvoiceDetailsModel()
            {
                InvoiceLineItems = await this.GetInvoiceLineItemsAsync(invoiceId, customerName, "Azure")
            };

            return this.PartialView(invoiceDetailsModel);
        }

        /// <summary>
        /// Handles the index view request.
        /// </summary>
        /// <returns>A view containing the InvoicesModel model.</returns>
        public async Task<ActionResult> Index()
        {
            InvoicesModel invoicesModel = new InvoicesModel()
            {
                Invoices = await this.Service.PartnerOperations.GetInvoicesAsync()
            };

            return this.View(invoicesModel);
        }

        /// <summary>
        /// Gets a list of customers included in the specified invoice.
        /// </summary>
        /// <param name="invoiceId">The invoice identifier.</param>
        /// <returns>
        /// A list of customers included in the specified invoice.
        /// </returns>
        [HttpGet]
        public async Task<JsonResult> GetCustomers(string invoiceId)
        {
            List<string> customers;
            List<InvoiceLineItem> lineItems;

            if (string.IsNullOrEmpty(invoiceId))
            {
                throw new ArgumentNullException(nameof(invoiceId));
            }

            try
            {
                customers = new List<string>();
                lineItems = await this.GetInvoiceLineItemsAsync(invoiceId);

                customers.AddRange(
                    lineItems
                        .Where(x => x is DailyUsageLineItem)
                        .Cast<DailyUsageLineItem>()
                        .Select(x => x.CustomerCompanyName));

                customers.AddRange(
                    lineItems
                        .Where(x => x is LicenseBasedLineItem)
                        .Cast<LicenseBasedLineItem>()
                        .Select(x => x.CustomerName));

                customers.AddRange(
                    lineItems
                        .Where(x => x is UsageBasedLineItem)
                        .Cast<UsageBasedLineItem>()
                        .Select(x => x.CustomerCompanyName));

                return this.Json(customers.Distinct(), JsonRequestBehavior.AllowGet);
            }
            finally
            {
                customers = null;
                lineItems = null;
            }
        }

        /// <summary>
        /// Handles the Details view request.
        /// </summary>
        /// <param name="invoiceId">The invoice identifier.</param>
        /// <returns>The HTML template for the details page.</returns>
        public ActionResult Details(string invoiceId)
        {
            if (string.IsNullOrEmpty(invoiceId))
            {
                return this.RedirectToAction("Index", "Invoices");
            }

            InvoiceDetailsModel invoiceDetailsModel = new InvoiceDetailsModel()
            {
                InvoiceId = invoiceId,
            };

            return this.View(invoiceDetailsModel);
        }

        /// <summary>
        /// Exports specific line items from a given invoice.
        /// </summary>
        /// <param name="invoiceId">The invoice identifier.</param>
        /// <param name="customerName">Name of the customer.</param>
        /// <param name="providerType">Type of the provider.</param>
        /// <returns>A CSV file containing the invoice details for the specified customer.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="invoiceId"/> is empty or null.
        /// or
        /// <paramref name="customerName"/> is empty or null.
        /// or
        /// <paramref name="providerType"/> is empty or null.
        /// </exception>
        public async Task<FileContentResult> ExportCustomer(string invoiceId, string customerName, string providerType)
        {
            byte[] data;

            invoiceId.AssertNotEmpty(nameof(invoiceId));
            customerName.AssertNotEmpty(nameof(customerName));
            providerType.AssertNotEmpty(nameof(providerType));

            try
            {
                if (providerType.Equals("Azure", StringComparison.OrdinalIgnoreCase))
                {
                    data = await this.GetUsageRecordsAsync(invoiceId, customerName);
                }
                else
                {
                    data = await this.GetLicensedRecordsAsync(invoiceId, customerName);
                }

                return this.File(data.ToArray(), "text/csv", $"Invoice-{invoiceId}-{customerName}-{providerType}.csv");
            }
            finally
            {
                data = null;
            }
        }

        /// <summary>
        /// Handles the request for the OfficeDetails partial view.
        /// </summary>
        /// <param name="customerName">Name of the customer.</param>
        /// <param name="invoiceId">The invoice identifier.</param>
        /// <returns>A partial view containing the InvoiceDetailsModel model.</returns>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="customerName"/> is empty or null.
        /// or 
        /// <paramref name="invoiceId"/> is empty or null.
        /// </exception>
        public async Task<PartialViewResult> OfficeDetails(string customerName, string invoiceId)
        {
            customerName.AssertNotEmpty(nameof(customerName));
            invoiceId.AssertNotEmpty(nameof(invoiceId));

            InvoiceDetailsModel invoiceDetailsModel = new InvoiceDetailsModel()
            {
                InvoiceLineItems = await this.GetInvoiceLineItemsAsync(invoiceId, customerName, "Office")
            };

            return this.PartialView(invoiceDetailsModel);
        }

        /// <summary>
        /// Gets a collection of resources representing line items in the specified invoice.
        /// </summary>
        /// <param name="invoiceId">The invoice identifier.</param>
        /// <returns>A collection of line items for the specified invoice.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="invoiceId"/> is empty or null.
        /// </exception>
        private async Task<List<InvoiceLineItem>> GetInvoiceLineItemsAsync(string invoiceId)
        {
            Invoice invoice;
            List<InvoiceLineItem> lineItems;
            List<InvoiceLineItem> data;
            string cacheName;

            invoiceId.AssertNotEmpty(nameof(invoiceId));

            try
            {
                cacheName = $"{invoiceId}_LineItems";
                lineItems = MemoryCache.Default[cacheName] as List<InvoiceLineItem>;

                if (lineItems != null)
                {
                    return lineItems;
                }

                invoice = await this.Service.PartnerOperations.GetInvoiceAsync(invoiceId);
                lineItems = new List<InvoiceLineItem>();

                foreach (InvoiceDetail detail in invoice.InvoiceDetails)
                {
                    data = await this.Service.PartnerOperations
                        .GetInvoiceLineItemsAsync(invoiceId, detail.BillingProvider, detail.InvoiceLineItemType);

                    lineItems.AddRange(data);
                }

                MemoryCache.Default[cacheName] = lineItems;

                return lineItems;
            }
            finally
            {
                lineItems = null;
            }
        }

        /// <summary>
        /// Gets a list of invoice line items from the specified invoice.
        /// </summary>
        /// <param name="invoiceId">Identifier of the invoice.</param>
        /// <param name="customerName">Name of the customer.</param>
        /// <param name="providerType">Type of subscriptions (Azure or Office).</param>
        /// <returns>A list of <see cref="InvoiceLineItem"/>s that represents the invoice line items.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="invoiceId"/> is empty or null.
        /// or
        /// <paramref name="customerName"/> is empty or null.
        /// or
        /// <paramref name="providerType"/> is empty or null.
        /// </exception>
        private async Task<List<InvoiceLineItem>> GetInvoiceLineItemsAsync(string invoiceId, string customerName, string providerType)
        {
            List<InvoiceLineItem> items;

            invoiceId.AssertNotEmpty(nameof(invoiceId));
            customerName.AssertNotEmpty(nameof(customerName));
            providerType.AssertNotEmpty(nameof(providerType));

            try
            {
                if (providerType.Equals("Azure", StringComparison.OrdinalIgnoreCase))
                {
                    items = await this.GetInvoiceLineItemsAsync(invoiceId);

                    return items
                        .Where(x => x is UsageBasedLineItem)
                        .Cast<UsageBasedLineItem>()
                        .Where(x => x.CustomerCompanyName.Equals(customerName, StringComparison.OrdinalIgnoreCase))
                        .ToList<InvoiceLineItem>();
                }
                else if (providerType.Equals("Office", StringComparison.OrdinalIgnoreCase))
                {
                    items = await this.GetInvoiceLineItemsAsync(invoiceId);

                    return items
                        .Where(x => x is LicenseBasedLineItem)
                        .Cast<LicenseBasedLineItem>()
                        .Where(x => x.CustomerName.Equals(customerName, StringComparison.OrdinalIgnoreCase))
                        .ToList<InvoiceLineItem>();
                }
                else
                {
                    return new List<InvoiceLineItem>();
                }
            }
            finally
            {
                items = null;
            }
        }

        /// <summary>
        /// Gets the license based entries from the specified invoice.
        /// </summary>
        /// <param name="invoiceId">Identifier of the invoice.</param>
        /// <param name="customerName">Name of the customer.</param>
        /// <returns>An array of <see cref="byte"/>s that contains the line items.</returns>
        private async Task<byte[]> GetLicensedRecordsAsync(string invoiceId, string customerName)
        {
            List<InvoiceLineItem> data;
            List<LicenseBasedLineItem> items;

            invoiceId.AssertNotEmpty(nameof(invoiceId));
            customerName.AssertNotEmpty(nameof(customerName));

            try
            {
                data = await this.GetInvoiceLineItemsAsync(invoiceId, customerName, "Office");
                items = data.Cast<LicenseBasedLineItem>().ToList();

                using (MemoryStream stream = new MemoryStream())
                {
                    using (TextWriter textWriter = new StreamWriter(stream))
                    {
                        using (CsvWriter writer = new CsvWriter(textWriter))
                        {
                            writer.WriteRecords(items);
                            writer.NextRecord();
                            textWriter.Flush();
                            return stream.ToArray();
                        }
                    }
                }
            }
            finally
            {
                items = null;
            }
        }

        /// <summary>
        /// Gets the usage records for the specific customer.
        /// </summary>
        /// <param name="invoiceId">Identifier for the invoice.</param>
        /// <param name="customerName">Name of the customer.</param>
        /// <returns>An array of <see cref="byte"/>s that contains invoice details.</returns>
        private async Task<byte[]> GetUsageRecordsAsync(string invoiceId, string customerName)
        {
            List<InvoiceLineItem> data;
            List<UsageBasedLineItem> items;

            invoiceId.AssertNotEmpty(nameof(invoiceId));
            customerName.AssertNotEmpty(nameof(customerName));

            try
            {
                data = await this.GetInvoiceLineItemsAsync(invoiceId, customerName, "Azure");
                items = data.Cast<UsageBasedLineItem>().ToList();

                using (MemoryStream stream = new MemoryStream())
                {
                    using (TextWriter textWriter = new StreamWriter(stream))
                    {
                        using (CsvWriter writer = new CsvWriter(textWriter))
                        {
                            writer.WriteRecords(items);
                            writer.NextRecord();
                            textWriter.Flush();
                            return stream.ToArray();
                        }
                    }
                }
            }
            finally
            {
                data = null;
                items = null;
            }
        }
    }
}