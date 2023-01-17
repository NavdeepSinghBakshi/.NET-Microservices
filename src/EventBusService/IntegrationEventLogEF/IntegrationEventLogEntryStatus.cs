using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntegrationEventLogEF
{
    [Table("Tbl_IntegrationStatus")]
    public partial class IntegrationEventLogEntryStatus
    {
        [Key]
        [Column("PK_IntegrationStatus_ID")]
        public int IntegrationStatusId { get; set; }

        [Column("EventId")]
        public Guid EventId { get; set; }

        [Column("Status")]
        public EventState SubscriberState { get; set; }
    }
}