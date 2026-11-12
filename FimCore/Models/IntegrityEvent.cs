namespace FimCore.Models
{
    public class IntegrityEvent
    {
        public string FilePath { get; set; }
        public string EventType { get; set; }
        public DateTime Timestamp { get; set; }
    }
}