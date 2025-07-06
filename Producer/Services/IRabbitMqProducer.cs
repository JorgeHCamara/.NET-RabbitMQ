using Core;

namespace Producer.Services
{
    public interface IRabbitMqProducer
    {
        void SendDefaultMessage(string message);
        void SendOrder(Order order);
    }
}
