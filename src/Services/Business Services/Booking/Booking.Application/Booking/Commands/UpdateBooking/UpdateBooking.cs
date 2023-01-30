using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace Booking.Application.Booking.Commands.UpdateBooking
{
    public class UpdateBooking : IRequest<Unit>
    {
        [JsonIgnore]
        public string BookingOrderId { get; set; }
        public string PaymentID { get; set; }
        public string NotificationID { get; set; }
    }
}
