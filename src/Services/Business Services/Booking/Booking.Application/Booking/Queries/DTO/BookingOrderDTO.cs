using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Booking.Queries.DTO
{
    public class BookingOrderDTO
    {
        public string BookingOrderId { get; set; }
        public string CustomerID { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }

        public ICollection<BookingOrderDetailDTO> BookingDetails { get;  set; }
    }

}
