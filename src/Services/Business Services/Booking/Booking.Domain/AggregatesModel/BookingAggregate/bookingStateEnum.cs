using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.AggregatesModel.BookingAggregate
{
    public enum bookingStateEnum
    {
        Pending = 0,
        Completed = 1,
        Canceled = 2
    }
}
