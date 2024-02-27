namespace RabbitMq_Messaging.Models
{
    public class InputMessageModel
    {
        public int FromId { get; set; }
        public int ToId { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
