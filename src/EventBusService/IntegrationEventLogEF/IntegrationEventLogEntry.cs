using EventBus.Events;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace IntegrationEventLogEF
{
    [Table("Tbl_IntegrationEventLogEntry")]
    public partial class IntegrationEventLogEntry
    {
        [Key]
        [Column("EventId")]
        public Guid EventId { get; set; }

        public string EventTypeName { get; set; }

        [NotMapped]
        public string EventTypeShortName => EventTypeName.Split('.')?.Last();

        [NotMapped]
        public IntegrationEvent IntegrationEvent { get; set; }

        public string State { get; set; }

        public int TimesSent { get; set; }

        public DateTime CreationTime { get; set; }

        public string Content { get; set; }

        public string SubscriberState { get; set; }

        public IntegrationEventLogEntry DeserializeJsonContent(Type type)
        {
            IntegrationEvent = JsonConvert.DeserializeObject(Content, type) as IntegrationEvent;
            return this;
        }
    }
}