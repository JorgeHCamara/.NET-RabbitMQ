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
            _producer.SendMessage(message);
            return Ok("Message sent successfully!");
        }
    }
}
