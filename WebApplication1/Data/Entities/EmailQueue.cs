namespace WebApplication1.Data.Entities
{
    public class EmailQueue
    {
        public Guid Id { get; set; }
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsSent { get; set; }
        public int RetryCount { get; set; }
        public DateTime CreateTime { get; set; }
        
    }
}
