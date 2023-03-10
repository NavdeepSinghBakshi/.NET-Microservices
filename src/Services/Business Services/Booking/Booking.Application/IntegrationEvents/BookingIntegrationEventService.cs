using Booking.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using EventBus.Abstractions;
using EventBus.Events;
using IntegrationEventLogEF;
using IntegrationEventLogEF.Services;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.IntegrationEvents
{
   public class BookingIntegrationEventService : IBookingIntegrationEventService
    {


        private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
        private readonly IEventBus _eventBus;
        private readonly BookingDbContext _bookingContext;
        private readonly IntegrationEventLogContext _eventLogContext;
        private readonly IIntegrationEventLogService _eventLogService;

        public BookingIntegrationEventService(IEventBus eventBus,
            BookingDbContext bookingContext,
            IntegrationEventLogContext eventLogContext,
            Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory)
        {
            _bookingContext = bookingContext ?? throw new ArgumentNullException(nameof(bookingContext));
            _eventLogContext = eventLogContext ?? throw new ArgumentNullException(nameof(eventLogContext));
            _integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _eventLogService = _integrationEventLogServiceFactory(_bookingContext.Database.GetDbConnection());
        }

        public async Task PublishEventsThroughEventBusAsync()
        {
            var pendindLogEvents = await _eventLogService.RetrieveEventLogsPendingToPublishAsync();
            foreach (var logEvt in pendindLogEvents)
            {
                try
                {
                    await _eventLogService.MarkEventAsInProgressAsync(logEvt.EventId);
                    _eventBus.Publish(logEvt.IntegrationEvent);
                    await _eventLogService.MarkEventAsPublishedAsync(logEvt.EventId);
                }
                catch (Exception)
                {
                    await _eventLogService.MarkEventAsFailedAsync(logEvt.EventId);
                }
            }
        }

        public async Task AddAndSaveEventAsync(IntegrationEvent evt)
        {
            //await _eventLogService.SaveEventAsync(evt, _bookingContext.GetCurrentTransaction.GetDbTransaction());
            await Task.CompletedTask;
        }
    }
}
