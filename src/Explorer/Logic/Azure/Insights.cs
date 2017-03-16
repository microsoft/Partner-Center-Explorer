// -----------------------------------------------------------------------
// <copyright file="Insights.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic.Azure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.Insights;
    using Microsoft.Azure.Insights.Models;
    using Rest;
    using Rest.Azure;
    using Rest.Azure.OData;

    /// <summary>
    /// Facilitates interactions with the Azure Insights in order to expose Azure health information.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class Insights : IDisposable
    {
        /// <summary>
        /// Name of the resource provider to be queried. 
        /// </summary>
        private const string ResourceProviderName = "Azure.Health";

        /// <summary>
        /// Provides the ability to interact with the Azure Monitor API. 
        /// </summary>
        private readonly IInsightsClient client;

        /// <summary>
        /// The subscription identifier that should be utilized in querying the health.
        /// </summary>
        private readonly string subscriptionId;

        /// <summary>
        /// A flag indicating whether or not this object has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Insights"/> class.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="token">Valid JSON Web Token (JWT).</param>
        /// <exception cref="System.ArgumentNullException">
        /// subscriptionId
        /// or
        /// token
        /// </exception>
        public Insights(string subscriptionId, string token)
        {
            subscriptionId.AssertNotEmpty(nameof(subscriptionId));
            token.AssertNotEmpty(nameof(token));

            this.client = new InsightsClient(new TokenCredentials(token));
            this.subscriptionId = subscriptionId;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets a list of health events for subscription.
        /// </summary>
        /// <returns>A list of health events for the given subscription.</returns>
        public async Task<List<IHealthEvent>> GetHealthEventsAsync()
        {
            DateTime queryEndDate;
            DateTime queryStartDate;
            IPage<EventData> events;
            ODataQuery<EventData> query;

            try
            {
                this.client.SubscriptionId = this.subscriptionId;

                queryEndDate = DateTime.UtcNow;
                queryStartDate = DateTime.UtcNow.AddMonths(-1);

                query = new ODataQuery<EventData>(FilterString.Generate<IncidentEvent>(
                    eventData => (eventData.EventTimestamp >= queryStartDate)
                    && (eventData.EventTimestamp <= queryEndDate)
                    && (eventData.ResourceProvider == ResourceProviderName)));

                events = await this.client.Events.ListAsync(query);

                return events.Select(x => new AzureHealthEvent
                {
                    Description = x.Description,
                    EventTimestamp = x.EventTimestamp,
                    ResourceGroupName = x.ResourceGroupName,
                    ResourceId = x.ResourceId,
                    ResourceProviderName = x.ResourceProviderName.Value,
                    ResourceType = x.ResourceType.Value,
                    Status = x.Status.LocalizedValue
                }).ToList<IHealthEvent>();
            }
            finally
            {
                events = null;
                query = null;
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.client.Dispose();

            if (disposing)
            {
                GC.SuppressFinalize(this);
            }

            this.disposed = true;
        }
    }
}