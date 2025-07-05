using System;

namespace dotnet_service.DTOs
{
    public class LogDto
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Level { get; set; }
        public DateTime Timestamp { get; set; }
    }
}