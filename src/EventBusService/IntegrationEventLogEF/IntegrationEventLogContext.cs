using Microsoft.EntityFrameworkCore;

namespace IntegrationEventLogEF
{
    public class IntegrationEventLogContext : DbContext
    {
        public IntegrationEventLogContext(string connectionString) : base(GetOptions(connectionString))
        {
        }

        private static DbContextOptions GetOptions(string connectionsting)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionsting).Options;
        }

        public DbSet<IntegrationEventLogEntry> IntegrationEventLogs { get; set; }

        public DbSet<IntegrationEventLogEntryStatus> IntegrationEventLogEntryStatuses { get; set; }
    }
}