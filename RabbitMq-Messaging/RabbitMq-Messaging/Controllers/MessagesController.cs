using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMq_Messaging.Models;
using RabbitMq_Messaging.Options;
using System.Text;

namespace RabbitMq_Messaging.Controllers
{
    [Route("api/[controller]")]
    public class MessagesController : Controller
    {
        private readonly RabbitMqConfiguration _config;
        private readonly ConnectionFactory _factory;

        public MessagesController(IOptions<RabbitMqConfiguration> option)
        {
            _config = option.Value;

            _factory = new ConnectionFactory
            {
                HostName = _config.Host
            };
        }

        // POST: MessagesController/Create
        [HttpPost]
        public IActionResult PostMessage([FromBody] InputMessageModel message)
        {
            using (var connection = _factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: _config.Queue,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                    );

                    var stringfiedMessage = JsonConvert.SerializeObject(message);
                    var bytesMessage = Encoding.UTF8.GetBytes(stringfiedMessage);

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: _config.Queue,
                        basicProperties: null,
                        body: bytesMessage
                    );

                }
            }
            return Ok();
        }
    }
}
