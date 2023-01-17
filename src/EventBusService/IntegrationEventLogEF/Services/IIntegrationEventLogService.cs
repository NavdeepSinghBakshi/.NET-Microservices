using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace IntegrationEventLogEF.Services
{
    public interface IIntegrationEventLogService
    {
        Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync();

        Task<IntegrationEventLogEntry> RetrieveEventLogsPendingToPublishAsync(Guid eventId);

        Task SaveEventAsync(IntegrationEventLogEntry eventLogEntry);

        Task MarkEventAsPublishedAsync(Guid eventId);

        Task MarkEventAsSubscribeAsync(Guid eventId);

        Task MarkEventAsInProgressAsync(Guid eventId);

        Task MarkEventAsFailedAsync(Guid eventId);

        Task MarkEventAsSubscribeFailedAsync(Guid eventId);

        string GetContent(Guid eventId);

        Task<List<IntegrationEventLogEntry>> GetEventLogs();

        Task<IntegrationEventLogEntry> GetEventLog(Guid eventId);
    }
}