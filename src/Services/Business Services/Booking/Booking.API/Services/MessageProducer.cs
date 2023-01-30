using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Booking.API.Services
{
    public class MessageProducer : IMessageProducer
    {
        public void SendingMessage<T>(T message)
        {
            var factory = new ConnectionFactory()
            {
                HostName="localhost",
                UserName="test",
                Password="test",
                //VirtualHost="/"
            };
            var con = factory.CreateConnection();
            using var channel = con.CreateModel();
            channel.QueueDeclare("book3", durable: false, exclusive: false, autoDelete: false, arguments: null);
            var jsonString = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonString);

            channel.BasicPublish("", "book3", body: body);
        }
    }
}
