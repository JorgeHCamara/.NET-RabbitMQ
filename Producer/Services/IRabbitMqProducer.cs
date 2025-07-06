namespace Producer.Services
{
    public interface IRabbitMqProducer
    {
        void SendMessage(string message);
    }
}
