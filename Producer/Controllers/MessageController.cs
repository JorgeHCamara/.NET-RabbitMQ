using Core;
using Microsoft.AspNetCore.Mvc;
using Producer.Services;
using RabbitMQ.Client;

namespace Producer.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IRabbitMqProducer _producer;

        public MessageController(IRabbitMqProducer producer)
        {
            _producer = producer;
        }

        [HttpPost]
        public IActionResult Post([FromBody] string message)
        {
            _producer.SendDefaultMessage(message);
            return Ok("Message sent successfully!");
        }

        [HttpPost("send-order")]
        public IActionResult SendSampleOrder()
        {
            var user = new User(1, "Jorge", "jorge@email.com");
            var order = new Order(123, user);
            _producer.SendOrder(order);
            return Ok("Order sent!");
        }
    }
}
