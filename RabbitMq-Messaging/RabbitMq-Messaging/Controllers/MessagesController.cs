using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMq_Messaging.Models;
using System.Text;

namespace RabbitMq_Messaging.Controllers
{
    [Route("api/[controller]")]
    public class MessagesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ConnectionFactory _factory;
        private readonly string _queueName;
        private readonly string _hostName;      

        public MessagesController(IConfiguration configuration)
        {
            _queueName = _configuration.GetValue<string>("QueueName");
            _hostName = _configuration.GetValue<string>("HostName");
            _configuration = configuration;
            _factory = new ConnectionFactory
            {
                HostName = "localhost"
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
                        queue: _queueName,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                    );

                    var stringfiedMessage = JsonConvert.SerializeObject(message);
                    var bytesMessage = Encoding.UTF8.GetBytes(stringfiedMessage);

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: _queueName,
                        basicProperties: null,
                        body: bytesMessage
                    );

                }
            }
            return Ok();
        }
    }
}
