using System;
using System.ComponentModel.DataAnnotations;

namespace dotnet_service.Models
{
    public class LogEntry
    {
        [Key]
        public int Id { get; set; }
        public string Message { get; set; }
        public string Level { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
