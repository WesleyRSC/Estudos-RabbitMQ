namespace RabbitMq_Messaging.Interface
{
    public interface INotificationService
    {
        void NotifyUserById(int fromId, int toId, string content);
    }
}