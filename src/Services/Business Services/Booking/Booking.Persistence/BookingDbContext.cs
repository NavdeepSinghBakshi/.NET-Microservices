using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Domain.AggregatesModel.BookingAggregate;
using Microsoft.EntityFrameworkCore;

namespace Booking.Persistence
{
    public class BookingDbContext : DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<BookingOrder> Bookings { get; set; }

        public DbSet<BookingOrderDetail> BookingsDetails { get; set; }
    }
}
