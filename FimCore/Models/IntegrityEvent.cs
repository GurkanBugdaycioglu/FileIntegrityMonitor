namespace FimCore.Models
{
    public class IntegrityEvent
    {
        public string FilePath { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? OldHash { get; set; }
        public string? NewHash { get; set; }
    }
}