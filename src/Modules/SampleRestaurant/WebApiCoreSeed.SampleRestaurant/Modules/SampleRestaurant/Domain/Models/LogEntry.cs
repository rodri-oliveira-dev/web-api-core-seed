using System;
using WebApiCoreSeed.SampleRestaurant.Models.Enums;

namespace WebApiCoreSeed.SampleRestaurant.Models
{
    public class LogEntry : Entity
    {
        public int? EventId { get; set; }
        public string? Escopo { get; set; }
        public ELogLevel LogLevel { get; set; } = ELogLevel.Debug;
        public string Message { get; set; } = string.Empty;
        public DateTime? CreatedTime { get; set; } = DateTime.Now;
    }
}
