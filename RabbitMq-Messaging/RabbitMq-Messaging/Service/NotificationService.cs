using RabbitMq_Messaging.Interface;

namespace RabbitMq_Messaging.Service
{
    public class NotificationService : INotificationService
    {
        public void NotifyUserById(int fromId, int toId, string content) {
            Console.WriteLine($"From user: {fromId}");
            Console.WriteLine($"To user: {toId}");
            Console.Write($"Message: {content}");
        }
    }
}
