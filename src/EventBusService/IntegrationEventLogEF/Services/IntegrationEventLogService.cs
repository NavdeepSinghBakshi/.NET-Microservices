using EventBus.Events;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace IntegrationEventLogEF.Services
{
    public class IntegrationEventLogService : IIntegrationEventLogService
    {
        private readonly IntegrationEventLogContext integrationEventLogContext;
        private readonly List<Type> eventTypes;

        public IntegrationEventLogService(string connectionString)
        {

            this.integrationEventLogContext = new IntegrationEventLogContext(connectionString);
            this.eventTypes = Assembly.Load(Assembly.GetEntryAssembly().FullName)
                .GetTypes()
                .Where(t => t.Name.EndsWith(nameof(IntegrationEvent)))
                .ToList();
        }

        public async Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync()
        {
            return await integrationEventLogContext.IntegrationEventLogs
                .Where(e => e.State == EventState.NotPublished.ToString())
                .OrderBy(o => o.CreationTime)
                .Select(e => e.DeserializeJsonContent(eventTypes.Find(t => t.Name == e.EventTypeShortName)))
                .ToListAsync();
        }

        public Task<IntegrationEventLogEntry> RetrieveEventLogsPendingToPublishAsync(Guid eventId)
        {
            return Task.FromResult(integrationEventLogContext.IntegrationEventLogs
                .Where(e => e.EventId == eventId)
                .Select(e => e.DeserializeJsonContent(eventTypes.Find(t => t.Name == e.EventTypeShortName))).FirstOrDefault()
                );
        }

        public Task SaveEventAsync(IntegrationEventLogEntry eventLogEntry)
        {
            integrationEventLogContext.IntegrationEventLogs.Add(eventLogEntry);
            return integrationEventLogContext.SaveChangesAsync();

        }

        public Task MarkEventAsPublishedAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventState.Published);
        }

        public Task MarkEventAsSubscribeAsync(Guid eventId)
        {
            return UpdateSubscribeEventStatus(eventId, EventState.Subscribe);
        }

        public Task MarkEventAsInProgressAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventState.InProgress);
        }

        public Task MarkEventAsFailedAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventState.PublishedFailed);
        }

        public Task MarkEventAsSubscribeFailedAsync(Guid eventId)
        {
            return UpdateSubscribeEventStatus(eventId, EventState.SubscribeFailed);
        }

        public string GetContent(Guid eventId)
        {
            var content = integrationEventLogContext.IntegrationEventLogs.Where(w => w.EventId == eventId).Select(s => s.Content).FirstOrDefault();
            return content;
        }

        private Task UpdateEventStatus(Guid eventId, EventState status)
        {
            var eventLogEntry = integrationEventLogContext.IntegrationEventLogs.FirstOrDefault(ie => ie.EventId == eventId);
            if (eventLogEntry != null)
            {
                eventLogEntry.State = status.ToString();
                if (status == EventState.InProgress)
                    eventLogEntry.TimesSent++;
            }
            integrationEventLogContext.IntegrationEventLogs.Update(eventLogEntry);
            var eventStatus = integrationEventLogContext.IntegrationEventLogEntryStatuses.FirstOrDefault(e => e.EventId == eventId);
            if (eventStatus != null)
            {
                eventStatus.SubscriberState = status;
            }
            else
            {
                integrationEventLogContext.IntegrationEventLogEntryStatuses.Add(new IntegrationEventLogEntryStatus
                {
                    EventId = eventId,
                    SubscriberState = status
                });
            }

            return integrationEventLogContext.SaveChangesAsync();
        }

        private Task UpdateSubscribeEventStatus(Guid eventId, EventState status)
        {
            var eventLogEntry = integrationEventLogContext.IntegrationEventLogs.FirstOrDefault(ie => ie.EventId == eventId);
            if (eventLogEntry != null)
            {
                eventLogEntry.SubscriberState = status.ToString();
            }
            var eventStatus = integrationEventLogContext.IntegrationEventLogEntryStatuses.FirstOrDefault(e => e.EventId == eventId);
            if (eventStatus != null)
            {
                eventStatus.SubscriberState = status;
            }
            else
            {
                integrationEventLogContext.IntegrationEventLogEntryStatuses.Add(new IntegrationEventLogEntryStatus
                {
                    EventId = eventId,
                    SubscriberState = status
                });
            }

            integrationEventLogContext.SaveChanges();
            return Task.CompletedTask;
        }

        public Task<List<IntegrationEventLogEntry>> GetEventLogs()
        {
            var queries = from log in integrationEventLogContext.IntegrationEventLogs
                          join status in integrationEventLogContext.IntegrationEventLogEntryStatuses on log.EventId equals status.EventId into evt
                          from eventStatus in evt.DefaultIfEmpty()
                          select new IntegrationEventLogEntry
                          {
                              EventId = log.EventId,
                              Content = log.Content,
                              CreationTime = log.CreationTime,
                              EventTypeName = log.EventTypeName,
                              IntegrationEvent = log.IntegrationEvent,
                              State = log.State,
                              TimesSent = log.TimesSent,
                              SubscriberState = eventStatus != null ? eventStatus.SubscriberState.ToString() : EventState.NotSubscribe.ToString()
                          };
            return Task.FromResult(queries.OrderByDescending(o => o.CreationTime).ToList());
        }

        public Task<IntegrationEventLogEntry> GetEventLog(Guid eventId)
        {
            var queries = from log in integrationEventLogContext.IntegrationEventLogs
                          join status in integrationEventLogContext.IntegrationEventLogEntryStatuses on log.EventId equals status.EventId into evt
                          from eventStatus in evt.DefaultIfEmpty()
                          where log.EventId == eventId
                          select new IntegrationEventLogEntry
                          {
                              EventId = log.EventId,
                              Content = log.Content,
                              CreationTime = log.CreationTime,
                              EventTypeName = log.EventTypeName,
                              IntegrationEvent = log.IntegrationEvent,
                              State = log.State,
                              TimesSent = log.TimesSent,
                              SubscriberState = eventStatus != null ? eventStatus.SubscriberState.ToString() : EventState.NotSubscribe.ToString()
                          };
            return Task.FromResult(queries.FirstOrDefault());
        }
    }
}