using System;
using Restaurante.IO.Business.Models.Enums;

namespace Restaurante.IO.Business.Models
{
    public class LogginEntity : Entity
    {
        public int? EventId { get; set; }
        public string Escopo { get; set; }
        public ELogLevel LogLevel { get; set; } = ELogLevel.Debug;
        public string Message { get; set; }
        public DateTime? CreatedTime { get; set; } = DateTime.Now;
    }
}
