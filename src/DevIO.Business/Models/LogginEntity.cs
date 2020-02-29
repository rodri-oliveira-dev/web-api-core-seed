using System;
using Microsoft.Extensions.Logging;

namespace Restaurante.IO.Business.Models
{
    public class LogginEntity : Entity
    {
        public int? EventId { get; set; }
        public string Escopo { get; set; }
        public LogLevel LogLevel { get; set; } = LogLevel.Debug;
        public string Message { get; set; }
        public DateTime? CreatedTime { get; set; } = DateTime.Now;
    }
}