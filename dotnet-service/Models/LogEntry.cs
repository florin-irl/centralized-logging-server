using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotnet_service.Models
{
    [Table("logs")]
    public class LogEntry
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("message")]
        public string Message { get; set; }
        [Column("level")]
        public string Level { get; set; }
        [Column("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
